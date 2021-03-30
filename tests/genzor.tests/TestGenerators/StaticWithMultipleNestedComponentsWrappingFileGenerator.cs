using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class StaticWithMultipleNestedComponentsWrappingFileGenerator : ComponentBase
	{
		public static readonly string NestedFileName = Guid.NewGuid().ToString();

		public string Name { get; } = string.Empty;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.AddContent(0, (RenderFragment)FirstLevelGenericParentComponent);
		}

		private void FirstLevelGenericParentComponent(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericParentComponent>(0);
			builder.AddAttribute(1, nameof(GenericParentComponent.ChildContent), (RenderFragment)SecondLevelGenericParentComponent);
			builder.CloseComponent();
		}

		private void SecondLevelGenericParentComponent(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericParentComponent>(0);
			builder.AddAttribute(1, nameof(GenericParentComponent.ChildContent), (RenderFragment)FileComponent);
			builder.CloseComponent();
		}

		private void FileComponent(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericFileGenerator>(0);
			builder.AddAttribute(1, nameof(GenericFileGenerator.Name), NestedFileName);
			builder.CloseComponent();
		}
	}
}