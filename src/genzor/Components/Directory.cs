using System;
using Genzor.FileSystem;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.Components
{
	/// <summary>
	/// Represents a directory that should be added to the <see cref="IFileSystem"/>.
	/// </summary>
	public class Directory : ComponentBase, IDirectoryComponent
	{
		/// <inheritdoc/>
		[Parameter] public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the child content to add to the directory.
		/// </summary>
		/// <remarks>
		/// Only components of type <see cref="IDirectoryComponent"/> or
		/// <see cref="IFileComponent"/> can be added to a directory.
		/// All other content will be ignored.
		/// </remarks>
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
