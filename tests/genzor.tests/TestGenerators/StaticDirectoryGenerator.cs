using Genzor.Components;
using Microsoft.AspNetCore.Components;

namespace Genzor.TestGenerators
{
	public class StaticDirectoryGenerator : ComponentBase, IDirectoryComponent
	{
		public string Name { get; } = "DirectoryName";
	}
}
