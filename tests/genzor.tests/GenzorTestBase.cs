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
