using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class TwoDirectoryGenerator : ComponentBase
	{
		public static readonly string FirstDirectoryName = Guid.NewGuid().ToString();
		public static readonly string SecondDirectoryName = Guid.NewGuid().ToString();

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericDirectoryGenerator>(0);
			builder.AddAttribute(1, nameof(GenericDirectoryGenerator.Name), FirstDirectoryName);
			builder.CloseComponent();
			builder.OpenComponent<GenericDirectoryGenerator>(10);
			builder.AddAttribute(11, nameof(GenericDirectoryGenerator.Name), SecondDirectoryName);
			builder.CloseComponent();
		}
	}
}
