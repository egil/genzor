using System;
using System.Runtime.Serialization;
using Genzor.Components;

namespace Genzor
{
	[Serializable]
	public sealed class InvalidGeneratorComponentContentException : Exception
	{
		private InvalidGeneratorComponentContentException(string? message) : base(message)
		{ }

		private InvalidGeneratorComponentContentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		public static InvalidGeneratorComponentContentException CreateUnexpectedDirectoryException(string directoryName)
			=> new InvalidGeneratorComponentContentException(
				$"A directory component ({nameof(IDirectoryComponent)}) cannot be the child of a file component ({nameof(IFileComponent)}). Name of misplaced directory: {directoryName}");
	}
}
