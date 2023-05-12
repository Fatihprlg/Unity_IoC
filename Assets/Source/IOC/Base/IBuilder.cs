
using System.Collections.Generic;

namespace IOC
{
    internal interface IBuilder
    {
        public void Build(Container container, List<AssemblyField> assemblies);
        public void MapClasses();
    }
}