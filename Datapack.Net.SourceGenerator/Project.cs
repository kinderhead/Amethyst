using System;
using System.Collections.Generic;

namespace Datapack.Net.SourceGenerator
{
    public readonly record struct Project
    {
        public readonly string Name;
        public readonly string Namespace;
        public readonly MCFunctions[] Functions;

        public Project(string name, string ns, List<MCFunctions> functions)
        {
            Name = name;
            Functions = [.. functions];
            Namespace = ns;
        }
    }

    public readonly record struct MCFunctions
    {
        public readonly string Name;
        public readonly (string, string)[] Arguments;

        public MCFunctions(string name, List<(string, string)> args)
        {
            Name = name;
            Arguments = [.. args];
        }
    }
}
