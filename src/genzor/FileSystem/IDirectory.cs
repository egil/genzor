using System.Collections.Generic;

namespace Genzor.FileSystem
{
	public interface IDirectory : IFileSystemItem
	{
		public IReadOnlyList<IFileSystemItem> Items { get; }
	}
}
