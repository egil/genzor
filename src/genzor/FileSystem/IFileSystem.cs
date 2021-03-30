using System.Collections.Generic;

namespace Genzor.FileSystem
{
	/// <summary>
	/// Represents a abstraction for a file system, which <see cref="GenzorRenderer"/> will
	/// add <see cref="IDirectory"/> and <see cref="IFile{TContent}"/> types to.
	/// </summary>
	public interface IFileSystem
	{
		/// <summary>
		/// Adds the <paramref name="item"/> to the root of the file system.
		/// </summary>
		/// <param name="item">Item to add to the file system.</param>
		void AddItem(IFileSystemItem item);
	}
}
