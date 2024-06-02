using System.Collections.Generic;

namespace Datapack.Net.SourceGenerator
{
    public readonly record struct MCFunction
    {
        public readonly string Name;
        public readonly string FullyQualifiedName;
        public readonly string ReturnType;
        public readonly (string, string)[] Arguments;
        public readonly bool Macro;

        public MCFunction(string name, string fullName, string returnType, List<(string, string)> args, bool macro)
        {
            Name = name;
            Arguments = [.. args];
            Macro = macro;
            FullyQualifiedName = fullName;
            ReturnType = returnType;
        }
    }
}
