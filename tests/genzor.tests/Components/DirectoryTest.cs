using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;
using Xunit.Abstractions;

namespace Genzor.Components
{
	public class DirectoryTest : GenzorTestBase
	{
		public DirectoryTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[AutoData]
		[Theory(DisplayName = "given directory name and no content, " +
							  "when generator is invoked, " +
							  "then a empty directory with specified name is added to file system")]
		public async Task Test001(string directoryName)
		{
			await Host.InvokeGeneratorAsync<Directory>(CreateParametersView((nameof(Directory.Name), directoryName)));

			FileSystem
				.Should()
				.ContainSingleDirectory()
				.WithName(directoryName)
				.And
				.WithoutItems();
		}

		[AutoData]
		[Theory(DisplayName = "given directory name and text file as content, " +
							  "when generator is invoked, " +
							  "then a directory with text file and specified name is added to file system")]
		public async Task Test002(string directoryName, string fileName)
		{
			await Host.InvokeGeneratorAsync<Directory>(
				CreateParametersView(
					(nameof(Directory.Name), directoryName),
					(nameof(TextFile.ChildContent), (RenderFragment)RenderTextFile)));

			FileSystem
				.Should()
				.ContainSingleDirectory()
				.WithName(directoryName)
				.And
				.WithItemsEquivalentTo(new[]
				{
					new { Name = fileName },
				});

			void RenderTextFile(RenderTreeBuilder builder)
			{
				builder.OpenComponent<TextFile>(0);
				builder.AddAttribute(1, nameof(TextFile.Name), fileName);
				builder.CloseComponent();
			}
		}

		[Fact(DisplayName = "given no directory name, " +
							"when generator is invoked, " +
							"then an argument exception is throw")]
		public async Task Test003()
		{
			Func<Task> throwingAction = () => Host.InvokeGeneratorAsync<Directory>();

			await throwingAction.Should()
				.ThrowAsync<ArgumentException>()
				.WithMessage("The Name parameter cannot be null or whitespace.");
		}
	}
}
