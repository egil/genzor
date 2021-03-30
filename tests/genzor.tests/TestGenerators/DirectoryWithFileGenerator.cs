using System;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class DirectoryWithFileGenerator : GenericDirectoryGenerator
	{
		public static readonly string ChildFileName = Guid.NewGuid().ToString();

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericFileGenerator>(0);
			builder.AddAttribute(1, nameof(GenericFileGenerator.Name), ChildFileName);
			builder.CloseComponent();
		}
	}
}
