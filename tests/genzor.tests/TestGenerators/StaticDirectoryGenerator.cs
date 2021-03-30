using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;

namespace Genzor.TestGenerators
{
	public class StaticDirectoryGenerator : ComponentBase, IDirectoryComponent
	{
		public static readonly string NameText = Guid.NewGuid().ToString();

		public string Name { get; } = NameText;
	}
}
