namespace Genzor.FileSystem
{
	public interface IFile : IFileSystemItem
	{
		/// <summary>
		/// Gets the content of the file.
		/// </summary>
		string Content { get; }
	}
}
