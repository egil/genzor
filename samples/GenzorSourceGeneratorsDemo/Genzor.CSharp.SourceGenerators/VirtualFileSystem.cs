using System;
using System.Collections;
using System.Collections.Generic;
using Genzor.FileSystem;

namespace Genzor.CSharp.SourceGenerators
{
	internal class VirtualFileSystem : IFileSystem, IEnumerable<(string PathAndName, string Content)>
	{
		private List<IFileSystemItem> items = new();

		public void AddItem(IFileSystemItem item)
		{
			items.Add(item);
		}

		public IEnumerator<(string PathAndName, string Content)> GetEnumerator()
		{
			var result = new List<(string PathAndName, string Content)>();

			foreach (var item in items)
			{
				result.AddRange(AddFiles(string.Empty, item));
			}

			return result.GetEnumerator();
		}

		private static IReadOnlyList<(string PathAndName, string Content)> AddFiles(string path, IFileSystemItem item)
		{
			return item switch
			{
				IDirectory directory => AddDirectory(path, directory),
				IFile<string> textFile => AddTextFile(path, textFile),
				_ => throw new NotImplementedException($"Unsupported file system item {item.GetType().FullName}"),
			};
		}

		private static IReadOnlyList<(string PathAndName, string Content)> AddDirectory(string path, IDirectory directory)
		{
			var result = new List<(string PathAndName, string Content)>();
			var dirPath = $"{path}_{directory.Name}";

			foreach (var item in directory)
			{
				result.AddRange(AddFiles(dirPath, item));
			}

			return result;
		}

		private static IReadOnlyList<(string PathAndName, string Content)> AddTextFile(string path, IFile<string> file)
		{
			var fullPath = $"{path}_{file.Name}";
			return new (string PathAndName, string Content)[] { (fullPath, file.Content) };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
