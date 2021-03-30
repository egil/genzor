using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class GenericTextComponent : ComponentBase
	{
		[Parameter] public string Text { get; set; } = string.Empty;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.AddContent(0, Text);
		}
	}
}
