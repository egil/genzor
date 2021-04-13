using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genzor.CSharp.SourceGenerators.Components;

namespace Genzor.CSharp.SourceGenerators
{
    public interface IUsedTypesCollection
    {
		void Add(TypeInfo typeInfo);
    }
}
