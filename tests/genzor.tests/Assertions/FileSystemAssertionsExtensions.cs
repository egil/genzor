using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Genzor.Assertions;
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

		public DirectoryAssertions ContainSingleDirectory(string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			var directory = Subject.Root
				.Should()
				.ContainSingle(because, becauseArgs)
				.Subject
				.Should()
				.BeAssignableTo<IDirectory>(because, becauseArgs)
				.Subject;

			return new DirectoryAssertions(directory);
		}

		public AndWhichConstraint<FileSystemAssertions, IEnumerable<IDirectory>> HaveDirectories(int count, string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			AndConstraint<Collections.GenericCollectionAssertions<IDirectory>> files = Subject.Root.OfType<IDirectory>()
				.Should()
				.HaveCount(count, because, becauseArgs);

			return new AndWhichConstraint<FileSystemAssertions, IEnumerable<IDirectory>>(this, files.And.Subject);
		}

		public FileAssertions<string> ContainSingleTextFile(string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			var file = Subject.Root
				.Should()
				.ContainSingle(because, becauseArgs)
				.Subject
				.Should()
				.BeAssignableTo<IFile<string>>(because, becauseArgs)
				.Subject;

			return new FileAssertions<string>(file);
		}

		public AndWhichConstraint<FileSystemAssertions, IEnumerable<IFile<string>>> HaveTextFiles(int count, string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			AndConstraint<Collections.GenericCollectionAssertions<IFile<string>>> files = Subject.Root.OfType<IFile<string>>()
				.Should()
				.HaveCount(count, because, becauseArgs);

			return new AndWhichConstraint<FileSystemAssertions, IEnumerable<IFile<string>>>(this, files.And.Subject);
		}
	}
}
