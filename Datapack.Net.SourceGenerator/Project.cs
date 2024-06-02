using System;
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
    }

    public readonly record struct MCFunction
    {
        public readonly string Name;
        public readonly (string, string)[] Arguments;

        public MCFunction(string name, List<(string, string)> args)
        {
            Name = name;
            Arguments = [.. args];
        }
    }
}
