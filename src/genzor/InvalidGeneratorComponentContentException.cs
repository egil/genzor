using System;
using System.Runtime.Serialization;
using Genzor.Components;

namespace Genzor
{
	/// <summary>
	/// Represents an exception that is thrown when an invalid state is detected in render tree generated
	/// by the <see cref="GenzorRenderer"/> when it invokes/renders generator components.
	/// </summary>
	[Serializable]
	public sealed class InvalidGeneratorComponentContentException : Exception
	{
		private InvalidGeneratorComponentContentException(string? message) : base(message)
		{ }

		private InvalidGeneratorComponentContentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		internal static InvalidGeneratorComponentContentException CreateUnexpectedDirectoryException(string directoryName)
			=> new InvalidGeneratorComponentContentException(
				$"A directory component ({nameof(IDirectoryComponent)}) cannot be the child of a file component ({nameof(IFileComponent)}). Name of misplaced directory: {directoryName}");
	}
}
