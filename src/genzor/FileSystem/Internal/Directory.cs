using System;
using System.Collections;
using System.Collections.Generic;

namespace Genzor.FileSystem.Internal
{
	internal class Directory : IDirectory
	{
		private readonly IReadOnlyList<IFileSystemItem> items;

		public string Name { get; }

		public int Count => items.Count;

		public IFileSystemItem this[int index] => items[index];

		public Directory(string name, IReadOnlyList<IFileSystemItem> items)
		{
			Name = name;
			this.items = items;
		}

		public IEnumerator<IFileSystemItem> GetEnumerator() => items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
	}
}
