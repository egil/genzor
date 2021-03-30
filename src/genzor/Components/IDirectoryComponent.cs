using Genzor.FileSystem;
using Microsoft.AspNetCore.Components;

namespace Genzor.Components
{
	/// <summary>
	/// Represents a component that when rendered will result in a <see cref="IDirectory"/>
	/// being added to the <see cref="IFileSystem"/>. Any child components of
	/// type <see cref="IDirectoryComponent"/> or <see cref="IFileComponent"/> will
	/// be added to the <see cref="IDirectory"/> as <see cref="IDirectory"/>
	/// or <see cref="IFile{TContent}"/> respectively.
	/// </summary>
	public interface IDirectoryComponent : IComponent
	{
		/// <summary>
		/// Gets the name of the directory the component renders.
		/// </summary>
		string Name { get; }
	}
}
