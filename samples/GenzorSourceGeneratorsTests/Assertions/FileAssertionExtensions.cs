using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Primitives;
using Genzor.FileSystem;

namespace Genzor.Assertions
{
	public static class FileAssertionsExtensions
	{
		public static FileAssertions<T> Should<T>(this IFile<T> file)
		{
			return new FileAssertions<T>(file);
		}
	}

	public class FileAssertions<T> : ReferenceTypeAssertions<IFile<T>, FileAssertions<T>>
	{
		protected override string Identifier { get; } = "file";

		public FileAssertions(IFile<T> subject) : base(subject)
		{
		}

		public AndConstraint<FileAssertions<T>> WithName(string expectedName, string because = "", params object[] becauseArgs)
		{
			Subject.Name.Should().Be(expectedName, because, becauseArgs);
			return new AndConstraint<FileAssertions<T>>(this);
		}

		public AndConstraint<FileAssertions<T>> WithContent(T expectedContent, string because = "", params object[] becauseArgs)
		{
			Subject.Content.Should().Be(expectedContent, because, becauseArgs);
			return new AndConstraint<FileAssertions<T>>(this);
		}
	}
}
