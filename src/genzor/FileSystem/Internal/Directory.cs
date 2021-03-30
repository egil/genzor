using System;
using System.Collections.Generic;

namespace Genzor.FileSystem.Internal
{
	internal class Directory : IDirectory
	{
		public string Name { get; }

		public IReadOnlyList<IFileSystemItem> Items { get; }

		public Directory(string name)
		{
			Name = name;
			Items = Array.Empty<IFileSystemItem>();
		}

		public void AddItem(IFileSystemItem item) => throw new System.NotImplementedException();
	}
}
