using System;
using System.Collections.Generic;
using Genzor.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Genzor
{
	public abstract class GenzorTestBase : IDisposable
	{
		private bool disposedValue;

		protected GenzorHost Host { get; }

		protected FakeFileSystem FileSystem { get; }

		protected GenzorTestBase(ITestOutputHelper outputHelper)
		{
			FileSystem = new FakeFileSystem();
			Host = new GenzorHost();
			Host.AddFileSystem(FileSystem);
			Host.AddLogging((builder) => builder.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Debug));
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

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Host?.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
