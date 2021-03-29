namespace Genzor.FileSystem
{
	public interface IFileSystemItem
	{
		/// <summary>
		/// Gets the name of the file or directory in a <see cref="IFileSystem" />.
		/// </summary>
		public string Name { get; }
	}
}
