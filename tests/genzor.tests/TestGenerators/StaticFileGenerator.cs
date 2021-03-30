using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class StaticFileGenerator : ComponentBase, IFileComponent
	{
		public static readonly string NameText = Guid.NewGuid().ToString();
		public static readonly string ContentText = Guid.NewGuid().ToString();

		public string Name { get; } = NameText;

		public string Content { get; } = ContentText;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
			=> builder.AddContent(0, Content);
	}
}
