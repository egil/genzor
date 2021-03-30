using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class TwoDirectoryGenerator : ComponentBase
	{
		public static readonly string FirstDirectoryName = nameof(FirstDirectoryName) + Guid.NewGuid().ToString();
		public static readonly string SecondDirectoryName = nameof(SecondDirectoryName) + Guid.NewGuid().ToString();

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<Directory>(0);
			builder.AddAttribute(1, nameof(Directory.Name), FirstDirectoryName);
			builder.CloseComponent();
			builder.OpenComponent<Directory>(10);
			builder.AddAttribute(11, nameof(Directory.Name), SecondDirectoryName);
			builder.CloseComponent();
		}
	}
}
