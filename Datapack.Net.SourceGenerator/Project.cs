using System;
using System.Collections;
using System.Collections.Generic;

namespace Datapack.Net.SourceGenerator
{
    public readonly record struct Project
    {
        public readonly string Name;
        public readonly string Namespace;
        public readonly MCFunction[] Functions;

        public Project(string name, string ns, List<MCFunction> functions)
        {
            Name = name;
            Functions = [.. functions];
            Namespace = ns;
        }

        public override int GetHashCode()
        {
            return unchecked(Name.GetHashCode() + Namespace.GetHashCode() + Functions.GetHashCode());
        }
    }
}
