using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class StaticFileGenerator : ComponentBase, IFileComponent
	{
		public string Name { get; } = "HelloWorld.txt";

		public string Content { get; } = "HELLO WORLD TEXT";

		protected override void BuildRenderTree(RenderTreeBuilder builder)
			=> builder.AddContent(0, Content);
	}
}
