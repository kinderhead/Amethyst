using System;
using System.Reflection;

namespace Datapack.Net.CubeLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimeObjectAttribute(string name) : Attribute
    {
        public readonly string Name = name;

        public static RuntimeObjectAttribute Get<T>()
        {
            return typeof(T).GetCustomAttribute<RuntimeObjectAttribute>() ?? throw new InvalidOperationException("Function does not have the DeclareMC attribute");
        }
    }
}
