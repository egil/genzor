using System.Collections.Generic;

namespace Genzor.FileSystem
{
	public interface IDirectory : IFileSystemItem
	{
		/// <summary>
		/// Gets the items in this directory, which can contain zero or more <see cref="IFileSystemItem"/>.
		/// </summary>
		public IReadOnlyList<IFileSystemItem> Items { get; }

		/// <summary>
		/// Adds the <paramref name="item"/> to this directory.
		/// </summary>
		/// <param name="item">Item to add to the directory.</param>
		void AddItem(IFileSystemItem item);
	}
}
