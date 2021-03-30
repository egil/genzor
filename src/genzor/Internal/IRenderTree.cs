using Microsoft.AspNetCore.Components.RenderTree;

namespace Genzor
{
	internal interface IRenderTree
	{
		ArrayRange<RenderTreeFrame> GetCurrentRenderTreeFrames(int componentId);
	}
}
