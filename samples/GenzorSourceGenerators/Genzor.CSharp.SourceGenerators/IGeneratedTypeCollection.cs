namespace Genzor.CSharp.SourceGenerators.Components
{
	public interface IGeneratedTypeCollection
	{
		void Add(TypeInfo typeInfo);

		TypeInfo GetByName(string @namespace, string name);
	}
}
