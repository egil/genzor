using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class StaticFileWithMultipleNestedComponentsWrappingItemsGenerator : ComponentBase, IDirectoryComponent
	{
		public static readonly string DirectoryName = Guid.NewGuid().ToString();
		public static readonly string NestedFileName = nameof(NestedFileName) + Guid.NewGuid().ToString();
		public static readonly string NestedDirectoryName = nameof(NestedDirectoryName) + Guid.NewGuid().ToString();

		public string Name { get; } = DirectoryName;

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
			builder.OpenComponent<TextFile>(0);
			builder.AddAttribute(1, nameof(TextFile.Name), NestedFileName);
			builder.CloseComponent();
			builder.OpenComponent<Directory>(10);
			builder.AddAttribute(11, nameof(Directory.Name), NestedDirectoryName);
			builder.CloseComponent();
		}
	}
}
