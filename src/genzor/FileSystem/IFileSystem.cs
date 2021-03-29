using System.Collections.Generic;

namespace Genzor.FileSystem
{
	public interface IFileSystem
	{
		public IReadOnlyList<IFileSystemItem> Root { get; }

		void AddItem(IFileSystemItem file);
	}
}
