using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.CSharp.SourceGenerators.Components
{
	public class Property<Type> : ComponentBase
	{
		private string typeName = "";
		[CascadingParameter] protected Namespace Namespace { get; set; } = default!;
		[CascadingParameter] protected IGeneratedTypeCollection GeneratedTypes { get; set; } = default!;
		[CascadingParameter] protected IUsedTypesCollection UsedTypes { get; set; } = default!;
		[Parameter] public string Modifiers { get; set; } = "public";
		[Parameter] public string Name { get; set; }
		[Parameter] public Type? GenType { get; set; }
		[Parameter] public Type? ClrType { get; set; }
		[Parameter] public bool HasGet { get; set; } = true;
		[Parameter] public bool HasSet { get; set; } = true;

		protected override void OnInitialized()
		{
			if (GenType is string genType)
			{
				UsedTypes.Add(GeneratedTypes.GetByName(Namespace.Name, genType));
			}

			if(ClrType is not System.Type type)
			{
				type = typeof(Type);
			}

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
			var prop = $"{Modifiers} {typeName} {Name} {{ ";
			if (HasGet)
				prop += "get; ";
			if (HasSet)
				prop += "set; ";
			prop += "}";

			builder.AddMarkupContent(0, prop);
		}
	}
}
