namespace Genzor.FileSystem.Internal
{
	internal class TextFile : IFile<string>
	{
		public string Name { get; }

		public string Content { get; }

		public TextFile(string name, string content)
		{
			Name = name;
			Content = content;
		}
	}
}
