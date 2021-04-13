using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Genzor.CSharp.SourceGenerators.Components
{
	public class Project : ComponentBase, IGeneratedTypeCollection
	{
		private readonly Dictionary<string, TypeInfo> generatedTypes = new();

		[Parameter] public RenderFragment? ChildContent { get; set; }

		public void Add(TypeInfo typeInfo)
		{
			if (generatedTypes.TryGetValue(typeInfo.FullName, out var existing))
			{
				existing.Exists = true;
			}
			else
			{
				generatedTypes.Add(typeInfo.FullName, typeInfo);
			}
		}

		public TypeInfo GetByName(string @namespace, string name)
		{
			var result = new TypeInfo(@namespace, name, false);
			if (generatedTypes.TryGetValue(result.FullName, out var existing))
			{
				result = existing;
			}
			else
			{
				generatedTypes.Add(result.FullName, result);
			}
			return result;
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<CascadingValue<IGeneratedTypeCollection>>(0);
			builder.AddAttribute(1, nameof(CascadingValue<IGeneratedTypeCollection>.IsFixed), true);
			builder.AddAttribute(2, nameof(CascadingValue<IGeneratedTypeCollection>.Value), this);
			builder.AddAttribute(3, nameof(CascadingValue<IGeneratedTypeCollection>.ChildContent), ChildContent);
			builder.CloseComponent();
		}
	}
}
