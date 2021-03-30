using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Genzor.FileSystem;
using Genzor.TestGenerators;
using Microsoft.AspNetCore.Components;
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

		[Fact(DisplayName = "when invoking generator that throws exception, " +
							"then the exception is re-thrown to caller")]
		public void Test102()
		{
			Func<Task> throwingAction = () => SUT.InvokeGeneratorAsync<ThrowingGenereator>();

			throwingAction
				.Should()
				.Throw<ThrowingGenereator.ThrowingGenereatorException>();
		}

		private static ParameterView CreateParametersView(params (string name, object value)[] parameters)
		{
			var dict = new Dictionary<string, object>(StringComparer.Ordinal);

			foreach (var pkv in parameters)
			{
				if (pkv.name == "ChildContent" && pkv.value is string text)
				{
					RenderFragment value = b => b.AddContent(0, text);
					dict.Add(pkv.name, value);
				}
				else
				{
					dict.Add(pkv.name, pkv.value);
				}
			}

			return ParameterView.FromDictionary(dict);
		}
	}
}
