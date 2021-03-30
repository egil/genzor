// Code copied from:
// - https://source.dot.net/#Microsoft.AspNetCore.Mvc.ViewFeatures/RazorComponents/HtmlRenderer.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Genzor.Components;
using Genzor.FileSystem;
using Genzor.FileSystem.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Genzor
{
	public sealed class GenzorRenderer : Renderer, IRenderTree
	{
		private readonly IFileSystem fileSystem;
		private readonly FileContentRenderTreeVisitor fileContentVisitor;

		public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

		public GenzorRenderer(IServiceProvider services, ILoggerFactory loggerFactory)
			: base(services, loggerFactory)
		{
			fileSystem = services.GetRequiredService<IFileSystem>();
			fileContentVisitor = new FileContentRenderTreeVisitor(this);
		}

		public Task InvokeGeneratorAsync<TComponent>(ParameterView? initialParameters = null)
			where TComponent : IComponent
		{
			return InvokeGeneratorAsync(typeof(TComponent), initialParameters ?? ParameterView.Empty);
		}

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
			return new Directory(component.Name, items);
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
			var file = new TextFile(component.Name, content);
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

		protected override void HandleException(Exception exception) => ExceptionDispatchInfo.Capture(exception).Throw();

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
