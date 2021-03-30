using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class TwoFileGenerator : ComponentBase
	{
		public static readonly string FirstFilesName = nameof(FirstFilesName) + Guid.NewGuid().ToString();
		public static readonly string SecondFilesName = nameof(SecondFilesName) + Guid.NewGuid().ToString();

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<TextFile>(0);
			builder.AddAttribute(1, nameof(TextFile.Name), FirstFilesName);
			builder.CloseComponent();
			builder.OpenComponent<TextFile>(10);
			builder.AddAttribute(11, nameof(TextFile.Name), SecondFilesName);
			builder.CloseComponent();
		}
	}
}
