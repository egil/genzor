using Genzor.FileSystem;
using Microsoft.AspNetCore.Components;

namespace Genzor.Components
{
	/// <summary>
	/// Represents a component that when rendered will result in a <see cref="IFile{TContent}"/>
	/// being added to the <see cref="IFileSystem"/>, or in a <see cref="IDirectory"/> containing it.
	/// </summary>
	public interface IFileComponent : IComponent
	{
		/// <summary>
		/// Gets the name of the file the component renders.
		/// </summary>
		string Name { get; }
	}
}
