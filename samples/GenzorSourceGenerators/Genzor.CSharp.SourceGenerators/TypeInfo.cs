using System;

namespace Genzor.CSharp.SourceGenerators
{
	public class TypeInfo : IEquatable<TypeInfo>
	{
		public string Name { get; } = string.Empty;

		public string FullName => $"{Namespace}.{Name}";

		public string Namespace { get; } = string.Empty;

		public bool Exists { get; internal set; } = false;

		public TypeInfo(string @namespace, string name, bool exists)
		{
			Namespace = @namespace;
			Name = name;
			Exists = exists;							
		}

		public override bool Equals(object obj)
			=> obj is TypeInfo other && Equals(other);

		public bool Equals(TypeInfo other)
			=> FullName.Equals(other.FullName, StringComparison.Ordinal);

		public override int GetHashCode()
			=> FullName.GetHashCode();
	}
}
