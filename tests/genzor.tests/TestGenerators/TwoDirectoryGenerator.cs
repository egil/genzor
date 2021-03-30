using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.TestGenerators
{
	public class TwoDirectoryGenerator : ComponentBase
	{
		public const string FirstDirectoryName = "DIR 1";
		public const string SecondDirectoryName = "DIR 2";

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
