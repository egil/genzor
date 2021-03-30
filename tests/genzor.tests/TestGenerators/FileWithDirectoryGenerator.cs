using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class FileWithDirectoryGenerator : ComponentBase, IFileComponent
	{
		public static readonly string FileName = Guid.NewGuid().ToString();
		public static readonly string DirectoryName = Guid.NewGuid().ToString();

		public string Name { get; } = FileName;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<Directory>(0);
			builder.AddAttribute(1, nameof(Directory.Name), DirectoryName);
			builder.CloseComponent();
		}
	}
}
