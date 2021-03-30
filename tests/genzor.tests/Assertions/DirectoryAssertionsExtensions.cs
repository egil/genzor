using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Genzor.Assertions;
using Genzor.FileSystem;

namespace FluentAssertions
{
	public static class DirectoryAssertionsExtensions
	{
		public static DirectoryAssertions Should(this IDirectory subject)
		{
			return new DirectoryAssertions(subject);
		}
	}

	public class DirectoryAssertions : ReferenceTypeAssertions<IDirectory, DirectoryAssertions>
	{
		protected override string Identifier { get; } = "directory";

		public IDirectory Which => Subject;

		public DirectoryAssertions(IDirectory subject) : base(subject)
		{
		}

		public AndConstraint<DirectoryAssertions> WithName(string expectedName, string because = "", params object[] becauseArgs)
		{
			Subject.Name.Should().Be(expectedName, because, becauseArgs);
			return new AndConstraint<DirectoryAssertions>(this);
		}

		public DirectoryAssertions ContainSingleDirectory(string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			var directory = Subject.Items
				.Should()
				.ContainSingle(because, becauseArgs)
				.Subject
				.Should()
				.BeAssignableTo<IDirectory>(because, becauseArgs)
				.Subject;

			return new DirectoryAssertions(directory);
		}

		public AndWhichConstraint<DirectoryAssertions, IEnumerable<IDirectory>> HaveDirectories(int count, string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			AndConstraint<Collections.GenericCollectionAssertions<IDirectory>> files = Subject.Items.OfType<IDirectory>()
				.Should()
				.HaveCount(count, because, becauseArgs);

			return new AndWhichConstraint<DirectoryAssertions, IEnumerable<IDirectory>>(this, files.And.Subject);
		}

		public FileAssertions<string> ContainSingleTextFile(string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			var file = Subject.Items
				.Should()
				.ContainSingle(because, becauseArgs)
				.Subject
				.Should()
				.BeAssignableTo<IFile<string>>(because, becauseArgs)
				.Subject;

			return new FileAssertions<string>(file);
		}

		public AndWhichConstraint<DirectoryAssertions, IEnumerable<IFile<string>>> HaveTextFiles(int count, string because = "", params object[] becauseArgs)
		{
			using var scope = new AssertionScope(Identifier);

			AndConstraint<Collections.GenericCollectionAssertions<IFile<string>>> files = Subject.Items.OfType<IFile<string>>()
				.Should()
				.HaveCount(count, because, becauseArgs);

			return new AndWhichConstraint<DirectoryAssertions, IEnumerable<IFile<string>>>(this, files.And.Subject);
		}
	}
}
