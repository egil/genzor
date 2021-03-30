using Genzor.Components;
using Microsoft.AspNetCore.Components;

namespace Genzor.TestGenerators
{
	public class GenericDirectoryGenerator : ComponentBase, IDirectoryComponent
	{
		[Parameter] public string Name { get; set; } = string.Empty;
	}
}
