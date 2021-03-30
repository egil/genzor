using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class DirectoryWithNoneFileSystemComponentGenerator : ComponentBase, IFileComponent
	{
		public static readonly Type ChildComponent = typeof(GenericParentComponent);

		public string Name { get; } = string.Empty;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent(0, ChildComponent);
			builder.CloseComponent();
		}
	}
}
