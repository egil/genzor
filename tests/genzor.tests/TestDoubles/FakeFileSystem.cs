using System.Collections.Generic;
using Genzor.FileSystem;

namespace Genzor
{
	internal class FakeFileSystem : IFileSystem
	{
		private readonly List<IFileSystemItem> items = new();

		public IReadOnlyList<IFileSystemItem> Root => items;

		public void AddItem(IFileSystemItem file)
		{
			items.Add(file);
		}
	}
}
