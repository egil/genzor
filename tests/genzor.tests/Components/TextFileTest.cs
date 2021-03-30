using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Genzor.Components
{
	public class TextFileTest : GenzorTestBase
	{
		public TextFileTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[AutoData]
		[Theory(DisplayName = "given file name and no content, " +
							  "when generator is invoked, " +
							  "then a empty file with specified name is added to file system")]
		public async Task Test001(string fileName)
		{
			await Host.InvokeGeneratorAsync<TextFile>(CreateParametersView((nameof(TextFile.Name), fileName)));

			FileSystem
				.Should()
				.ContainSingleTextFile()
				.WithName(fileName)
				.And
				.WithContent(string.Empty);
		}

		[AutoData]
		[Theory(DisplayName = "given file name and content, " +
							  "when generator is invoked, " +
							  "then a file with content and specified name is added to file system")]
		public async Task Test002(string fileName, string content)
		{
			await Host.InvokeGeneratorAsync<TextFile>(
				CreateParametersView(
					(nameof(TextFile.Name), fileName),
					(nameof(TextFile.ChildContent), content)));

			FileSystem
				.Should()
				.ContainSingleTextFile()
				.WithName(fileName)
				.And
				.WithContent(content);
		}

		[Fact(DisplayName = "given no file name, " +
							"when generator is invoked, " +
							"then an argument exception is throw")]
		public async Task Test003()
		{
			Func<Task> throwingAction = () => Host.InvokeGeneratorAsync<TextFile>();

			await throwingAction.Should()
				.ThrowAsync<ArgumentException>()
				.WithMessage("The Name parameter cannot be null or whitespace.");
		}
	}
}
