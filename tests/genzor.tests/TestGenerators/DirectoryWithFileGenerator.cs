using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class DirectoryWithFileGenerator : ComponentBase, IDirectoryComponent
	{
		public static readonly string ChildFileName = Guid.NewGuid().ToString();

		public string Name { get; } = nameof(DirectoryWithFileGenerator);

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<TextFile>(0);
			builder.AddAttribute(1, nameof(TextFile.Name), ChildFileName);
			builder.CloseComponent();
		}
	}
}
