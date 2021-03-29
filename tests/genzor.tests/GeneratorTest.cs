using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public GeneratorTest(ITestOutputHelper outputHelper)
		{
			var services = new ServiceCollection()
				.AddLogging((builder) => builder.AddXUnit(outputHelper));

			services.AddSingleton<FakeFileSystem>();
			services.AddSingleton<IFileSystem>(s => s.GetRequiredService<FakeFileSystem>());
			services.AddSingleton<Generator>();

			Services = services.BuildServiceProvider();
		}

		[Fact(DisplayName = "when invoking generator with HelloWorldGenerator, " +
							"then a HelloWorld.txt file is added to file system")]
		public async Task Test001()
		{
			var fileSystem = Services.GetRequiredService<IFileSystem>();

			var sut = Services.GetRequiredService<Generator>();

			await sut.InvokeGeneratorAsync<HelloWorldGenerator>();

			fileSystem.Root
				.Should()
				.ContainSingle()
				.Subject
				.Should()
				.BeAssignableTo<IFile>()
				.Subject
				.Name
				.Should()
				.Be("HelloWorld.txt");
		}

		// passing parameters to generators
    }
}
