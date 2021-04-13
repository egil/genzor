using Microsoft.AspNetCore.Components;

namespace Genzor.CSharp.SourceGenerators.Components
{
	public abstract class GenzorComponentBase : ComponentBase
	{
		[Parameter] public bool Visible { get; set; } = true;

		protected override bool ShouldRender() => Visible;
	}
}
