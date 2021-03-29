using System;
using Genzor.Components;
using Microsoft.AspNetCore.Components;

namespace Genzor.TestGenerators
{
	public class ThrowingGenereator : ComponentBase, IFileComponent
	{
		public string Name { get; } = string.Empty;

		protected override void OnInitialized() => throw new ThrowingGenereatorException();

		public sealed class ThrowingGenereatorException : Exception { }
	}
}
