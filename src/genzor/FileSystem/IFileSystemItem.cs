namespace Genzor.FileSystem
{
	/// <summary>
	/// Represents a generic file system item, i.e. either an
	/// <see cref="IDirectory"/> or an <see cref="IFile{TContent}"/>.
	/// </summary>
	public interface IFileSystemItem
	{
		/// <summary>
		/// Gets the name of the file or directory in a <see cref="IFileSystem" />.
		/// </summary>
		public string Name { get; }
	}
}
