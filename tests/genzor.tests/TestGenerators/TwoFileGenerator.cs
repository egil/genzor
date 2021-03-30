using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class TwoFileGenerator : ComponentBase
	{
		public static readonly string FirstFilesName = Guid.NewGuid().ToString();
		public static readonly string SecondFilesName = Guid.NewGuid().ToString();

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericFileGenerator>(0);
			builder.AddAttribute(1, nameof(GenericFileGenerator.Name), FirstFilesName);
			builder.CloseComponent();
			builder.OpenComponent<GenericFileGenerator>(10);
			builder.AddAttribute(11, nameof(GenericFileGenerator.Name), SecondFilesName);
			builder.CloseComponent();
		}
	}
}
