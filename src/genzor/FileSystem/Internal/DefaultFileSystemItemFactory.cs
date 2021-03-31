using System;
using System.Collections.Generic;

namespace Genzor.FileSystem.Internal
{
	internal class DefaultFileSystemItemFactory : IFileSystemItemFactory
	{
		public IDirectory CreateDirectory(string name, IReadOnlyList<IFileSystemItem> items)
			=> new Directory(name, items);

		public IFile<TContent> CreateFile<TContent>(string name, TContent content)
			=> content switch
			{
				string c => (IFile<TContent>)new TextFile(name, c),
				_ => throw new InvalidOperationException($"Support for files with content of type {typeof(TContent)} is not yet implemented.")
			};
	}
}
