using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class StaticFileWithChildComponentGenerator : ComponentBase, IFileComponent
	{
		public static readonly string ChildComponentText = Guid.NewGuid().ToString();

		public string Name { get; } = nameof(StaticFileWithChildComponentGenerator);

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<GenericTextComponent>(0);
			builder.AddAttribute(1, nameof(GenericTextComponent.Text), ChildComponentText);
			builder.CloseComponent();
		}
	}
}
