using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class StaticFileWithMultipleNestedChildComponentsGenerator : ComponentBase, IFileComponent
	{
		public static readonly string Child1ComponentText = Guid.NewGuid().ToString();
		public static readonly string Child2ComponentText = Guid.NewGuid().ToString();

		public string Name { get; } = nameof(StaticFileWithChildComponentGenerator);

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericParentComponent>(0);
			builder.AddAttribute(1, nameof(GenericParentComponent.ChildContent), (RenderFragment)Child1ComponentContent);
			builder.CloseComponent();
			builder.OpenComponent<GenericParentComponent>(10);
			builder.AddAttribute(11, nameof(GenericParentComponent.ChildContent), (RenderFragment)Child2ComponentContent);
			builder.CloseComponent();
		}

		private void Child1ComponentContent(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericTextComponent>(0);
			builder.AddAttribute(1, nameof(GenericTextComponent.Text), Child1ComponentText);
			builder.CloseComponent();
		}

		private void Child2ComponentContent(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericTextComponent>(0);
			builder.AddAttribute(1, nameof(GenericTextComponent.Text), Child2ComponentText);
			builder.CloseComponent();
		}
	}
}
