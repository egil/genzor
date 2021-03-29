using System;
using System.Threading.Tasks;
using FluentAssertions;
using Genzor.FileSystem;
using Genzor.TestGenerators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Genzor
{
	public class GeneratorTest
	{
		private IServiceProvider Services { get; }

		private Generator SUT => Services.GetRequiredService<Generator>();

		private IFileSystem FileSystem => Services.GetRequiredService<IFileSystem>();

		public GeneratorTest(ITestOutputHelper outputHelper)
		{
			var services = new ServiceCollection();
			services.AddLogging((builder) => builder.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Debug));
			services.AddSingleton<FakeFileSystem>();
			services.AddSingleton<IFileSystem>(s => s.GetRequiredService<FakeFileSystem>());
			services.AddSingleton<Generator>();

			Services = services.BuildServiceProvider();
		}

		[Fact(DisplayName = "given generator that creates a file, " +
							"when invoking generator, " +
							"then a file with expected name is added to file system")]
		public async Task Test001()
		{
			await SUT.InvokeGeneratorAsync<StaticFileWithContent>();

			FileSystem
				.Should()
				.ContainSingleFile()
				.Subject
				.Name
				.Should()
				.Be(new StaticFileWithContent().Name);
		}

		[Fact(DisplayName = "given generator that creates file with content, " +
							"when invoking generator, " +
							"then a file with expected content is added to file system")]
		public async Task Test002()
		{
			await SUT.InvokeGeneratorAsync<StaticFileWithContent>();

			FileSystem
				.Should()
				.ContainSingleFile()
				.Subject
				.Content
				.Should()
				.Be(new StaticFileWithContent().Content);
		}

		[Fact(DisplayName = "when invoking generator that throws exception, " +
							"then the exception is re-thrown to caller")]
		public void Test102()
		{
			Func<Task> throwingAction = () => SUT.InvokeGeneratorAsync<ThrowingGenereator>();

			throwingAction
				.Should()
				.Throw<ThrowingGenereator.ThrowingGenereatorException>();
		}

		// passing parameters to generators
	}
}
