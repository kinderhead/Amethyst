using System;
using System.Collections.Generic;

namespace Datapack.Net.SourceGenerator
{
    public readonly record struct RuntimeObject
    {
        public readonly string Name;
        public readonly string Namespace;
        public readonly MCFunction[] Methods;

        public RuntimeObject(string name, string ns, List<MCFunction> methods)
        {
            Name = name;
            Namespace = ns;
            Methods = [.. methods];
        }
    }
}
