using Microsoft.AspNetCore.Components;

namespace Genzor.Components
{
	public interface IDirectoryComponent : IComponent
	{
		/// <summary>
		/// Gets the name of the directory the component renders.
		/// </summary>
		string Name { get; }
	}
}
