using System;
using System.Collections.Generic;

namespace Datapack.Net.SourceGenerator
{
    public readonly record struct RuntimeObject
    {
        public readonly string Name;
        public readonly string Namespace;
        public readonly MCFunction[] Methods;
        public readonly RuntimeObjectProperty[] Properties;

        public RuntimeObject(string name, string ns, List<MCFunction> methods, List<RuntimeObjectProperty> props)
        {
            Name = name;
            Namespace = ns;
            Methods = [.. methods];
            Properties = [.. props];
        }
    }

    public readonly record struct RuntimeObjectProperty
    {
        public readonly string Name;
        public readonly string InternalName;
        public readonly string Type;
        public readonly bool IsObj;

        public RuntimeObjectProperty(string name, string internalName, string type, bool isObj)
        {
            Name = name;
            InternalName = internalName;
            Type = type;
            IsObj = isObj;
        }
    }
}
