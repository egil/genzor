using System.Collections.Generic;

namespace Genzor.FileSystem
{
	/// <summary>
	/// Represent a generated directory, which can hold
	/// zero or more <see cref="IFileSystemItem"/>.
	/// </summary>
	public interface IDirectory : IReadOnlyList<IFileSystemItem>, IFileSystemItem
	{
	}
}
