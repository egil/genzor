using Genzor.FileSystem;

namespace Genzor.FileSystem.Internal
{
	internal class File : IFile
	{
		public string Name { get; }

		public File(string name) => Name = name;
	}
}
