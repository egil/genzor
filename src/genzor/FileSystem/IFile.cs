namespace Genzor.FileSystem
{
	public interface IFile<out TContent> : IFileSystemItem
	{
		/// <summary>
		/// Gets the content of the file.
		/// </summary>
		TContent Content { get; }
	}
}
