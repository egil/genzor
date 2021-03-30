// Some of the code in this class is copied from:
// - https://source.dot.net/#Microsoft.AspNetCore.Mvc.ViewFeatures/RazorComponents/HtmlRenderer.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Genzor.Components;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Genzor
{
	internal class FileContentRenderTreeVisitor
	{
		private readonly IRenderTree renderTree;

		public FileContentRenderTreeVisitor(IRenderTree renderTree)
		{
			this.renderTree = renderTree;
		}

		public string GetTextContent(int componentId)
		{
			var frames = renderTree.GetCurrentRenderTreeFrames(componentId);
			var context = new HtmlRenderingContext();
			var newPosition = RenderFrames(context, frames, 0, frames.Count);
			Debug.Assert(newPosition == frames.Count, "All render frames for component was not processes.");
			return string.Join(null, context.Result);
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
			var childFrames = renderTree.GetCurrentRenderTreeFrames(frame.ComponentId);
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
				Debug.Assert(afterElement == position + frame.ElementSubtreeLength, "Unexpected position after processing element");
				return afterElement;
			}
			else
			{
				result.Add(">");
				result.Add("</");
				result.Add(frame.ElementName);
				result.Add(">");
				Debug.Assert(afterAttributes == position + frame.ElementSubtreeLength, "Unexpected position after processing element");
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
