using System;
using System.Runtime.Serialization;
using Genzor.Components;

namespace Genzor
{
	[Serializable]
	public sealed class InvalidFileComponentContentException : Exception
	{
		private InvalidFileComponentContentException(string? message) : base(message)
		{ }

		private InvalidFileComponentContentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }

		public static InvalidFileComponentContentException CreateDirectoryNotAllowed(string directoryName)
			=> new InvalidFileComponentContentException(
				$"A directory component ({nameof(IDirectoryComponent)}) cannot be the child of a file component ({nameof(IFileComponent)}). Name of misplaced directory: {directoryName}");
	}
}
