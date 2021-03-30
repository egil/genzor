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

		public AndConstraint<DirectoryAssertions> WithItemsEquivalentTo<TExpectation>(IEnumerable<TExpectation> expectation, string because = "", params object[] becauseArgs)
		{
			((IReadOnlyList<IFileSystemItem>)Subject).Should().BeEquivalentTo(expectation, because, becauseArgs);
			return new AndConstraint<DirectoryAssertions>(this);
		}

		public FileAssertions<string> ContainSingleTextFile(string because = "", params object[] becauseArgs)
		{
			var file = ((IReadOnlyList<IFileSystemItem>)Subject)
				.Should()
				.ContainSingle(because, becauseArgs)
				.Subject
				.Should()
				.BeAssignableTo<IFile<string>>(because, becauseArgs)
				.Subject;

			return new FileAssertions<string>(file);
		}
	}
}
