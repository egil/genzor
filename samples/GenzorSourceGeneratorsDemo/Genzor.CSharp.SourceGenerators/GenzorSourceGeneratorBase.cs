using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Genzor.CSharp.SourceGenerators
{
	public abstract class GenzorSourceGeneratorBase : ComponentBase, IGenzorSourceGenerator, ISourceGenerator
	{
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "Just prototyping for now.")]
		private static readonly DiagnosticDescriptor GeneratorRuntimeInfo =
			new(id: "GSG0001",
				title: "Generator runtime",
				messageFormat: "The generator '{0}' completed in '{1}' milliseconds",
				category: "GenzorSourceGenerator",
				DiagnosticSeverity.Info,
				isEnabledByDefault: true);

		[Parameter]
		public abstract GeneratorExecutionContext Context { get; set; }
		
		public void Initialize(GeneratorInitializationContext context)
		{
			// No initialization required
		}

		public void Execute(GeneratorExecutionContext context)
		{
			var generatorType = GetType();
			var runtimeMilliseconds = InvokeGenerator(generatorType, context);
			ReportingRuntime(generatorType, context, runtimeMilliseconds);
		}

		private static long InvokeGenerator(Type generatorType, GeneratorExecutionContext context)
		{
			var stopWatch = Stopwatch.StartNew();
			var fileSystem = new VirtualFileSystem();
			using (var host = new GenzorHost().AddFileSystem(fileSystem))
			{
				var dict = new Dictionary<string, object> { { "Context", context } };
				var generatorTask = host.Renderer.InvokeGeneratorAsync(generatorType, ParameterView.FromDictionary(dict));

				// Task should be completed already, unless the generator component
				// is doing async stuff in its async life cycle methods.
				generatorTask.Wait();

				// inject the created source into the users compilation
				foreach (var (PathAndName, Content) in fileSystem)
				{
					context.AddSource(PathAndName, SourceText.From(Content, Encoding.UTF8));
				}
			}
			stopWatch.Stop();
			return stopWatch.ElapsedMilliseconds;
		}

		private static void ReportingRuntime(Type generatorType, GeneratorExecutionContext context, long runtimeMilliseconds)
		{
			var runtimeDiag = Diagnostic.Create(
				GeneratorRuntimeInfo,
				Location.None,
				generatorType.Name,
				runtimeMilliseconds);

			context.ReportDiagnostic(runtimeDiag);
		}
	}
}
