using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class GenericFile : ComponentBase, IFileComponent
	{
		[Parameter] public string Name { get; set; } = string.Empty;

		[Parameter] public RenderFragment ChildContent { get; set; }

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.AddContent(0, ChildContent);
		}
	}
}
