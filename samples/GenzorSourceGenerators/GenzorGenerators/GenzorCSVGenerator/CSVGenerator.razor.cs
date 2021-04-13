using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvGenerator;
using Genzor.CSharp.SourceGenerators;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace GenzorSourceGenerators.GenzorGenerators.GenzorCSVGenerator
{
	public partial class CSVGenerator : GenzorSourceGeneratorBase
	{
		IEnumerable<CsvGeneratorOption> Options { get; set; } = Enumerable.Empty<CsvGeneratorOption>();

		[Parameter] public override GeneratorExecutionContext Context { get; set; }

		protected override void OnInitialized()
		{
			// get options
			Options = GetLoadOptions(Context);
		}

		static IEnumerable<CsvGeneratorOption> GetLoadOptions(GeneratorExecutionContext context)
		{
			foreach (AdditionalText file in context.AdditionalFiles)
			{
				if (Path.GetExtension(file.Path).Equals(".csv", StringComparison.OrdinalIgnoreCase))
				{
					// are there any options for it?
					context.AnalyzerConfigOptions.GetOptions(file).TryGetValue("build_metadata.additionalfiles.CsvLoadType", out string? loadTimeString);
					Enum.TryParse(loadTimeString, ignoreCase: true, out CsvLoadType loadType);

					context.AnalyzerConfigOptions.GetOptions(file).TryGetValue("build_metadata.additionalfiles.CacheObjects", out string? cacheObjectsString);
					bool.TryParse(cacheObjectsString, out bool cacheObjects);

					yield return new CsvGeneratorOption(
							file.GetText()?.ToString() ?? string.Empty,
							Path.GetFileNameWithoutExtension(file.Path),
							loadType,
							cacheObjects);
				}
			}
		}
	}
}
