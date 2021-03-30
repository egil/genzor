using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Genzor.FileSystem;
using Genzor.TestGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Genzor
{
	public class GenzorRendererTest : GenzorTestBase
	{
		public GenzorRenderer SUT => Services.GetRequiredService<GenzorRenderer>();

		public IFileSystem FileSystem => Services.GetRequiredService<IFileSystem>();

		public GenzorRendererTest(ITestOutputHelper outputHelper) : base(outputHelper) { }

		[Fact(DisplayName = "when invoking a generator which throws an exception, " +
							"then the exception is re-thrown to caller")]
		public void Test102()
		{
			Func<Task> throwingAction = () => SUT.InvokeGeneratorAsync<ThrowingGenereator>();

			throwingAction
				.Should()
				.Throw<ThrowingGenereator.ThrowingGenereatorException>();
		}

		[Fact(DisplayName = "given generator that creates a file, " +
							"when invoking generator, " +
							"then a generated file is added to file system")]
		public async Task Test001()
		{
			await SUT.InvokeGeneratorAsync<StaticFileGenerator>();

			FileSystem
				.Should()
				.ContainSingleFile()
				.Subject
				.Should()
				.BeEquivalentTo(new
				{
					Name = new StaticFileGenerator().Name,
					Content = new StaticFileGenerator().Content,
				});
		}

		[AutoData]
		[Theory(DisplayName = "given generator that takes parameters, " +
							  "when invoking generator with parameters, " +
							  "then parameters are passed to generator")]
		public async Task Test011(string filename, string content)
		{
			await SUT.InvokeGeneratorAsync<GenericFileGenerator>(
				CreateParametersView(
					("Name", filename),
					("ChildContent", content)));

			FileSystem
				.Should()
				.ContainSingleFile()
				.Subject
				.Should()
				.BeEquivalentTo(new
				{
					Name = filename,
					Content = content,
				});
		}

		[Fact(DisplayName = "given generator that creates multiple file, " +
							"when invoking generator, " +
							"then generated files is added to file system in generated order")]
		public async Task Test021()
		{
			await SUT.InvokeGeneratorAsync<TwoFileGenerator>();

			FileSystem
				.Should()
				.HaveFiles(2)
				.Which
				.Should()
				.BeEquivalentTo(new[]
				{
					new { Name = TwoFileGenerator.FirstFilesName },
					new { Name = TwoFileGenerator.SecondFilesName },
				});
		}

		[Fact(DisplayName = "given generator that creates a directory, " +
							"when invoking generator, " +
							"then generated directory is added to file system")]
		public async Task Test031()
		{
			await SUT.InvokeGeneratorAsync<StaticDirectoryGenerator>();

			FileSystem
				.Should()
				.ContainSingleDirectory()
				.Which
				.Should()
				.BeEquivalentTo(new
				{
					Name = new StaticDirectoryGenerator().Name,
				});
		}
	}
}
