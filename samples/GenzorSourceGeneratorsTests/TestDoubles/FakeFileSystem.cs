using System.Collections.Generic;
using Genzor.FileSystem;

namespace Genzor.TestDoubles
{
	public class FakeFileSystem : IFileSystem
	{
		private readonly List<IFileSystemItem> items = new();

		public IReadOnlyList<IFileSystemItem> Root => items;

		public void AddItem(IFileSystemItem item)
		{
			items.Add(item);
		}
	}
}
