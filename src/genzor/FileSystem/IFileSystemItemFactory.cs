using System.Collections.Generic;

namespace Genzor.FileSystem
{
	/// <summary>
	/// Represents an factory for <see cref="IFileSystemItem"/> types,
	/// that is used by the <see cref="GenzorRenderer"/> to create the
	/// file system items during generation/rendering.
	/// </summary>
	public interface IFileSystemItemFactory
	{
		/// <summary>
		/// Creates an <see cref="IFile{TContent}"/>.
		/// </summary>
		/// <typeparam name="TContent">The type of content in the file.</typeparam>
		/// <param name="name">The name of the file (relative, should not include the parent directory's paths.).</param>
		/// <param name="content">The content to add to the file.</param>
		/// <returns>The created <see cref="IFile{TContent}"/>.</returns>
		IFile<TContent> CreateFile<TContent>(string name, TContent content);

		/// <summary>
		/// Creates an <see cref="IDirectory"/>.
		/// </summary>
		/// <param name="name">The name of the directory (relative, should not include the parent directory's paths.).</param>
		/// <param name="items">The <see cref="IFileSystemItem"/> items in the directory.</param>
		/// <returns>The created <see cref="IDirectory"/>.</returns>
		IDirectory CreateDirectory(string name, IReadOnlyList<IFileSystemItem> items);
	}
}
