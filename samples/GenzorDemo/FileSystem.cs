using System;
using System.IO;
using Genzor.FileSystem;

namespace GenzorDemo
{
    class FileSystem : IFileSystem
    {
        private readonly DirectoryInfo rootDirectory;

        public FileSystem(DirectoryInfo rootDirectory) 
            => this.rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));

        public void AddItem(IFileSystemItem item)
            => AddItem(rootDirectory, item);

        private void AddItem(DirectoryInfo parent, IFileSystemItem item)
        {
            switch (item)
            {
                case IDirectory directory:
                    AddDirectory(parent, directory);
                    break;
                case IFile<string> textFile:
                    AddTextFile(parent, textFile);
                    break;
                default:
                    throw new NotImplementedException($"Unsupported file system item {item.GetType().FullName}");
            }
        }

        private void AddDirectory(DirectoryInfo parent, IDirectory directory)
        {
            var createdDirectory = parent.CreateSubdirectory(directory.Name);

            foreach (var item in directory)
                AddItem(createdDirectory, item);
        }

        private void AddTextFile(DirectoryInfo parent, IFile<string> file)
        {
            var fullPath = Path.Combine(parent.FullName, file.Name);
            File.WriteAllText(fullPath, file.Content);
        }
    }
}