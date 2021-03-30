// Code copied from:
// - https://source.dot.net/#Microsoft.AspNetCore.Mvc.ViewFeatures/RazorComponents/HtmlRenderer.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public sealed class GenzorRenderer : Renderer
	{
		private readonly IFileSystem fileSystem;

		public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

		public GenzorRenderer(IServiceProvider services, ILoggerFactory loggerFactory)
			: base(services, loggerFactory)
		{
			fileSystem = services.GetRequiredService<IFileSystem>();
		}

		public Task InvokeGeneratorAsync<TComponent>(ParameterView? initialParameters = null)
			where TComponent : IComponent
		{
			return InvokeGeneratorAsync(typeof(TComponent), initialParameters ?? ParameterView.Empty);
		}

		public async Task InvokeGeneratorAsync(Type componentType, ParameterView initialParameters)
		{
			var (id, component) = await Dispatcher.InvokeAsync(() => CreateInitialRenderAsync(componentType, initialParameters));

			AddItemsToFileSystem(id, component);
		}

		private void AddItemsToFileSystem(int componentId, IComponent component)
		{
			switch (component)
			{
				case IFileComponent fc:
					AddFileToFileSystem(componentId, fc);
					break;
				case IDirectoryComponent d:
					AddDirectoryToFileSystem(componentId, d);
					break;
			}

			var frames = GetCurrentRenderTreeFrames(componentId);

			for (var i = 0; i < frames.Count; i++)
			{
				ref var frame = ref frames.Array[i];

				if (frame.FrameType == RenderTreeFrameType.Component)
				{
					switch (frame.Component)
					{
						case IFileComponent fc:
							AddFileToFileSystem(frame.ComponentId, fc);
							break;
						case IDirectoryComponent d:
							AddDirectoryToFileSystem(frame.ComponentId, d);
							break;
					}
				}
			}
		}

		private void AddFileToFileSystem(int componentId, IFileComponent component)
		{
			var frames = GetCurrentRenderTreeFrames(componentId);
			var context = new HtmlRenderingContext();
			var newPosition = RenderFrames(context, frames, 0, frames.Count);
			Debug.Assert(newPosition == frames.Count, "All render frames for component was not processes.");
			var file = new TextFile(component.Name, string.Join(null, context.Result));
			fileSystem.AddItem(file);
		}

		private void AddDirectoryToFileSystem(int componentId, IDirectoryComponent component)
		{
			var file = new Directory(component.Name);
			fileSystem.AddItem(file);
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

		private int RenderFrames(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
		{
			var nextPosition = position;
			var endPosition = position + maxElements;
			while (position < endPosition)
			{
				nextPosition = RenderCore(context, frames, position);
				if (position == nextPosition)
				{
					throw new InvalidOperationException("We didn't consume any input.");
				}

				position = nextPosition;
			}

			return nextPosition;
		}

		private int RenderCore(
			HtmlRenderingContext context,
			ArrayRange<RenderTreeFrame> frames,
			int position)
		{
			ref var frame = ref frames.Array[position];
			switch (frame.FrameType)
			{
				case RenderTreeFrameType.Element:
					return RenderElement(context, frames, position);
				case RenderTreeFrameType.Attribute:
					throw new InvalidOperationException($"Attributes should only be encountered within {nameof(RenderElement)}");
				case RenderTreeFrameType.Text:
					context.Result.Add(frame.TextContent);
					return ++position;
				case RenderTreeFrameType.Markup:
					context.Result.Add(frame.MarkupContent);
					return ++position;
				case RenderTreeFrameType.Component when frame.Component is IDirectoryComponent dc:
					throw InvalidGeneratorComponentContentException.CreateUnexpectedDirectoryException(dc.Name);
				case RenderTreeFrameType.Component when frame.Component is not IDirectoryComponent:
					return RenderChildComponent(context, frames, position);
				case RenderTreeFrameType.Region:
					return RenderFrames(context, frames, position + 1, frame.RegionSubtreeLength - 1);
				case RenderTreeFrameType.ElementReferenceCapture:
				case RenderTreeFrameType.ComponentReferenceCapture:
					return ++position;
				default:
					throw new InvalidOperationException($"Invalid element frame type '{frame.FrameType}'.");
			}
		}

		private int RenderChildComponent(
			HtmlRenderingContext context,
			ArrayRange<RenderTreeFrame> frames,
			int position)
		{
			ref var frame = ref frames.Array[position];
			var childFrames = GetCurrentRenderTreeFrames(frame.ComponentId);
			RenderFrames(context, childFrames, 0, childFrames.Count);
			return position + frame.ComponentSubtreeLength;
		}

		private int RenderElement(
			HtmlRenderingContext context,
			ArrayRange<RenderTreeFrame> frames,
			int position)
		{
			ref var frame = ref frames.Array[position];
			var result = context.Result;
			result.Add("<");
			result.Add(frame.ElementName);
			var afterAttributes = RenderAttributes(context, frames, position + 1, frame.ElementSubtreeLength - 1, out var capturedValueAttribute);

			// When we see an <option> as a descendant of a <select>, and the option's "value" attribute matches the
			// "value" attribute on the <select>, then we auto-add the "selected" attribute to that option. This is
			// a way of converting Blazor's select binding feature to regular static HTML.
			if (context.ClosestSelectValueAsString != null
				&& string.Equals(frame.ElementName, "option", StringComparison.OrdinalIgnoreCase)
				&& string.Equals(capturedValueAttribute, context.ClosestSelectValueAsString, StringComparison.Ordinal))
			{
				result.Add(" selected");
			}

			var remainingElements = frame.ElementSubtreeLength + position - afterAttributes;
			if (remainingElements > 0)
			{
				result.Add(">");

				var isSelect = string.Equals(frame.ElementName, "select", StringComparison.OrdinalIgnoreCase);
				if (isSelect)
				{
					context.ClosestSelectValueAsString = capturedValueAttribute;
				}

				var afterElement = RenderChildren(context, frames, afterAttributes, remainingElements);

				if (isSelect)
				{
					// There's no concept of nested <select> elements, so as soon as we're exiting one of them,
					// we can safely say there is no longer any value for this
					context.ClosestSelectValueAsString = null;
				}

				result.Add("</");
				result.Add(frame.ElementName);
				result.Add(">");
				Debug.Assert(afterElement == position + frame.ElementSubtreeLength);
				return afterElement;
			}
			else
			{
				result.Add(">");
				result.Add("</");
				result.Add(frame.ElementName);
				result.Add(">");
				Debug.Assert(afterAttributes == position + frame.ElementSubtreeLength);
				return afterAttributes;
			}
		}

		private int RenderChildren(HtmlRenderingContext context, ArrayRange<RenderTreeFrame> frames, int position, int maxElements)
		{
			if (maxElements == 0)
			{
				return position;
			}

			return RenderFrames(context, frames, position, maxElements);
		}

		private static int RenderAttributes(
			HtmlRenderingContext context,
			ArrayRange<RenderTreeFrame> frames,
			int position,
			int maxElements,
			out string? capturedValueAttribute)
		{
			capturedValueAttribute = null;

			if (maxElements == 0)
			{
				return position;
			}

			var result = context.Result;

			for (var i = 0; i < maxElements; i++)
			{
				var candidateIndex = position + i;
				ref var frame = ref frames.Array[candidateIndex];
				if (frame.FrameType != RenderTreeFrameType.Attribute)
				{
					return candidateIndex;
				}

				if (frame.AttributeName.Equals("value", StringComparison.OrdinalIgnoreCase))
				{
					capturedValueAttribute = frame.AttributeValue as string;
				}

				switch (frame.AttributeValue)
				{
					case bool flag when flag:
						result.Add(" ");
						result.Add(frame.AttributeName);
						break;
					case string value:
						result.Add(" ");
						result.Add(frame.AttributeName);
						result.Add("=");
						result.Add("\"");
						result.Add(value);
						result.Add("\"");
						break;
					default:
						break;
				}
			}

			return position + maxElements;
		}

		private class HtmlRenderingContext
		{
			public List<string> Result { get; } = new List<string>();

			public string? ClosestSelectValueAsString { get; set; }
		}
	}
}
