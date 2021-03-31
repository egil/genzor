using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace Genzor.CSharp.SourceGenerators
{
	public interface IGenzorSourceGenerator
	{
		[Parameter] public GeneratorExecutionContext Context { get; set; }
	}
}
