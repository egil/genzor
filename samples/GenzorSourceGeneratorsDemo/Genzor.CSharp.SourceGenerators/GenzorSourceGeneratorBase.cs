using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Genzor.CSharp.SourceGenerators
{
	public abstract class GenzorSourceGeneratorBase : ComponentBase, IGenzorSourceGenerator, ISourceGenerator, IDisposable
	{
		private readonly VirtualFileSystem fileSystem;
		private readonly GenzorHost host;

		[Parameter]
		public abstract GeneratorExecutionContext Context { get; set; }

		public GenzorSourceGeneratorBase()
		{
			fileSystem = new VirtualFileSystem();
			host = new GenzorHost().AddFileSystem(fileSystem);
		}

		public void Execute(GeneratorExecutionContext context)
		{
			var dict = new Dictionary<string, object> { { "Context", context } };
			var generatorTask = host.Renderer.InvokeGeneratorAsync(GetType(), ParameterView.FromDictionary(dict));

			// Task should be completed already, unless the generator component
			// is doing async stuff in its async life cycle methods.
			generatorTask.Wait();

			// inject the created source into the users compilation
			foreach (var fileInfo in fileSystem)
			{
				context.AddSource(fileInfo.PathAndName, SourceText.From(fileInfo.Content, Encoding.UTF8));
			}
		}

		public void Initialize(GeneratorInitializationContext context)
		{
			// No initialization required
		}

		public void Dispose() => host.Dispose();
	}
}
