using System;
using System.Collections.Generic;
using Genzor.FileSystem;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Genzor
{
	public abstract class GenzorTestBase
	{
		protected IServiceProvider Services { get; }

		protected GenzorTestBase(ITestOutputHelper outputHelper)
		{
			var services = new ServiceCollection();
			services.AddLogging((builder) => builder.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Debug));
			services.AddSingleton<FakeFileSystem>();
			services.AddSingleton<IFileSystem>(s => s.GetRequiredService<FakeFileSystem>());
			services.AddSingleton<GenzorRenderer>();
			Services = services.BuildServiceProvider();
		}

		protected static ParameterView CreateParametersView(params (string name, object value)[] parameters)
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
