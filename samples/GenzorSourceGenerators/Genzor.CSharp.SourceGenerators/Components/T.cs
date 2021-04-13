using Genzor.CSharp.SourceGenerators;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.CSharp.SourceGenerators.Components
{
	public class UseGenType : ComponentBase
	{
		private TypeInfo usedType;

		[CascadingParameter] public Namespace Namespace { get; set; }
		[CascadingParameter] protected IUsedTypesCollection UsedTypes { get; set; }
		[CascadingParameter] public IGeneratedTypeCollection GeneratedTypes { get; set; }

		[Parameter] public string Name { get; set; }

		protected override void OnInitialized()
		{
			usedType = GeneratedTypes.GetByName(Namespace.Name, Name);
			UsedTypes.Add(usedType);
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.AddContent(0, usedType.Name);
		}
	}

	public class UseType<Type> : ComponentBase
	{
		private string typeName = "";

		[CascadingParameter] protected Namespace Namespace { get; set; }

		[CascadingParameter] protected IGeneratedTypeCollection GeneratedTypes { get; set; }

		[CascadingParameter] protected IUsedTypesCollection UsedTypes { get; set; }

		[Parameter] public string? GenType { get; set; }

		protected override void OnInitialized()
		{
			if (GenType is not null)
			{
				UsedTypes.Add(GeneratedTypes.GetByName(Namespace.Name, GenType));
			}

			var type = typeof(Type);
			UsedTypes.Add(new TypeInfo(type.Namespace, type.Name, true));

			if (type.IsGenericType && GenType is not null)
			{
				typeName = $"{type.Name.Substring(0, type.Name.IndexOf('`'))}<{GenType}>";
			}
			else
			{
				typeName = type.Name;
			}
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.AddMarkupContent(0, typeName);
		}
	}

	public sealed class GenType { }
}
