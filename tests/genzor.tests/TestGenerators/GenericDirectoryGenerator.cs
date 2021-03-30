using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class GenericDirectoryGenerator : ComponentBase, IDirectoryComponent
	{
		[Parameter] public string Name { get; set; } = string.Empty;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
		}
	}
}
