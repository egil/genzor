using System;
using Genzor.FileSystem;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.Components
{
	/// <summary>
	/// Represents a text file that should be added to the <see cref="IFileSystem"/>.
	/// </summary>
	public class TextFile : ComponentBase, IFileComponent
	{
		/// <inheritdoc/>
		[Parameter] public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the child content to add to the file.
		/// </summary>
		[Parameter] public RenderFragment? ChildContent { get; set; }

		/// <inheritdoc/>
		protected override void OnParametersSet()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				throw new ArgumentException("The Name parameter cannot be null or whitespace.");
			}
		}

		/// <inheritdoc/>
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder?.AddContent(0, ChildContent);
		}
	}
}
