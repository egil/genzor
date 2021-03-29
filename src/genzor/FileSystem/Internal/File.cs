namespace Genzor.FileSystem.Internal
{
	internal class File : IFile
	{
		public string Name { get; }

		public string Content { get; }

		public File(string name, string content)
		{
			Name = name;
			Content = content;
		}
	}
}
