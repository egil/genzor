// Some of the code in this class is copied from:
// - https://source.dot.net/#Microsoft.AspNetCore.Mvc.ViewFeatures/RazorComponents/HtmlRenderer.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Genzor.Components;
using Genzor.FileSystem;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FSDirectory = Genzor.FileSystem.Internal.Directory;
using FSTextFile = Genzor.FileSystem.Internal.TextFile;

namespace Genzor
{
	/// <summary>
	/// Represents a renderer for generator components. It will add any
	/// <see cref="IDirectoryComponent"/> or <see cref="IFileComponent"/> components
	/// to the <see cref="IFileSystem"/> registered with the <see cref="IServiceProvider"/>
	/// passed to it.
	/// </summary>
	public sealed class GenzorRenderer : Renderer, IRenderTree
	{
		private readonly IFileSystem fileSystem;
		private readonly FileContentRenderTreeVisitor fileContentVisitor;
		private readonly ILogger<GenzorRenderer> logger;

		/// <inheritdoc/>
		public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

		/// <summary>
		/// Initializes a new instance of the <see cref="GenzorRenderer"/> class.
		/// </summary>
		/// <param name="services">The <see cref="IServiceProvider "/> to be used when initializing components and get dependencies from.</param>
		/// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
		public GenzorRenderer(IServiceProvider services, ILoggerFactory loggerFactory)
			: base(services, loggerFactory)
		{
			fileSystem = services.GetRequiredService<IFileSystem>();
			fileContentVisitor = new FileContentRenderTreeVisitor(this);
			logger = loggerFactory.CreateLogger<GenzorRenderer>();
		}

		/// <summary>
		/// Invoke (render) the <typeparamref name="TComponent"/> generator and add
		/// any <see cref="IDirectoryComponent"/> or <see cref="IFileComponent"/> components
		/// to the <see cref="IFileSystem"/> registered with the <see cref="IServiceProvider"/>.
		/// </summary>
		/// <typeparam name="TComponent">The generator component to invoke.</typeparam>
		/// <param name="initialParameters">Optional parameters to pass to the generator.</param>
		/// <returns>A <see cref="Task"/> that completes when the generator finishes.</returns>
		public Task InvokeGeneratorAsync<TComponent>(ParameterView? initialParameters = null)
			where TComponent : IComponent
		{
			return InvokeGeneratorAsync(typeof(TComponent), initialParameters ?? ParameterView.Empty);
		}

		/// <summary>
		/// Invoke (render) the <paramref name="componentType"/> generator and add
		/// any <see cref="IDirectoryComponent"/> or <see cref="IFileComponent"/> components
		/// to the <see cref="IFileSystem"/> registered with the <see cref="IServiceProvider"/>.
		/// </summary>
		/// <param name="componentType">The type of generator component to invoke.</param>
		/// <param name="initialParameters">Optional parameters to pass to the generator.</param>
		/// <returns>A <see cref="Task"/> that completes when the generator finishes.</returns>
		public async Task InvokeGeneratorAsync(Type componentType, ParameterView initialParameters)
		{
			var (id, component) = await Dispatcher.InvokeAsync(() => CreateInitialRenderAsync(componentType, initialParameters));

			// If the rendered component is either a IFileComponent or a
			// IDirectoryComponent, this will get it and any of its children
			// added to the file system.
			// Otherwise it is just a generic component that can contain
			// any number of other file or directory components we traverse
			// to find all file or directory components and add them to the file system.
			if (TryGetItem(id, component, out var fileSystemItem))
			{
				fileSystem.AddItem(fileSystemItem);
			}
			else
			{
				foreach (var item in GetFileSystemItems(id))
				{
					fileSystem.AddItem(item);
				}
			}
		}

		private IDirectory GetDirectoryWithItems(int componentId, IDirectoryComponent component)
		{
			var items = GetFileSystemItems(componentId);
			return new FSDirectory(component.Name, items);
		}

		private List<IFileSystemItem> GetFileSystemItems(int parentComponentId)
		{
			var result = new List<IFileSystemItem>();
			var frames = GetCurrentRenderTreeFrames(parentComponentId);

			for (var i = 0; i < frames.Count; i++)
			{
				ref var frame = ref frames.Array[i];

				if (frame.FrameType == RenderTreeFrameType.Component)
				{
					if (TryGetItem(frame.ComponentId, frame.Component, out var fileSystemItem))
					{
						result.Add(fileSystemItem);
					}
					else
					{
						result.AddRange(GetFileSystemItems(frame.ComponentId));
					}
				}
			}

			return result;
		}

		private IFileSystemItem GetFile(int componentId, IFileComponent component)
		{
			var content = fileContentVisitor.GetTextContent(componentId);
			var file = new FSTextFile(component.Name, content);
			return file;
		}

		private bool TryGetItem(int componentId, IComponent component, [NotNullWhen(true)] out IFileSystemItem? fileSystemItem)
		{
			fileSystemItem = component switch
			{
				IFileComponent fc => GetFile(componentId, fc),
				IDirectoryComponent d => GetDirectoryWithItems(componentId, d),
				_ => null,
			};

			return fileSystemItem is not null;
		}

		/// <inheritdoc/>
		protected override void HandleException(Exception exception) => ExceptionDispatchInfo.Capture(exception).Throw();

		/// <inheritdoc/>
		protected override Task UpdateDisplayAsync(in RenderBatch renderBatch) => Task.CompletedTask;

		private async Task<(int ComponentId, IComponent Component)> CreateInitialRenderAsync(Type componentType, ParameterView initialParameters)
		{
			var component = InstantiateComponent(componentType);
			var componentId = AssignRootComponentId(component);
			await RenderRootComponentAsync(componentId, initialParameters);
			return (componentId, component);
		}

		ArrayRange<RenderTreeFrame> IRenderTree.GetCurrentRenderTreeFrames(int componentId)
			=> GetCurrentRenderTreeFrames(componentId);
	}
}
