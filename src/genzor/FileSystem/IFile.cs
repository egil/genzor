namespace Genzor.FileSystem
{
	/// <summary>
	/// Represents a file in a <see cref="IFileSystem" />
	/// that has content of type <typeparamref name="TContent"/>.
	/// </summary>
	/// <typeparam name="TContent">The content type of the content in the file.</typeparam>
	public interface IFile<out TContent> : IFileSystemItem
	{
		/// <summary>
		/// Gets the content of the file.
		/// </summary>
		TContent Content { get; }
	}
}
