using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Genzor.FileSystem;

namespace FluentAssertions
{
	public static class FileSystemAssertionsExtensions
	{
		public static FileSystemAssertions Should(this IFileSystem fileSystem)
		{
			return new FileSystemAssertions(fileSystem);
		}
	}

	public class FileSystemAssertions : ReferenceTypeAssertions<IFileSystem, FileSystemAssertions>
	{
		protected override string Identifier { get; } = "file system";

		public FileSystemAssertions(IFileSystem subject) : base(subject)
		{
		}

		public AndWhichConstraint<FileSystemAssertions, IFile> ContainSingleFile(string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			var file = Subject.Root
				.Should()
				.ContainSingle(because, becauseArgs)
				.Subject
				.Should()
				.BeAssignableTo<IFile>(because, becauseArgs)
				.Subject;

			return new AndWhichConstraint<FileSystemAssertions, IFile>(this, file);
		}
	}
}
