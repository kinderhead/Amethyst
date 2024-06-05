using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DeclareMCAttribute : Attribute
    {
        public readonly string Path;
        public readonly bool Returns;
        public readonly string[] Macros = [];

        public DeclareMCAttribute(string name, string[] macros)
        {
            Path = name;
            Returns = false;
            Macros = macros;
        }

        public DeclareMCAttribute(string name)
        {
            Path = name;
            Returns = false;
        }

        public static DeclareMCAttribute Get(Delegate func)
        {
            return Get(func.Method);
        }

        public static DeclareMCAttribute Get(MethodInfo func)
        {
            return func.GetCustomAttribute<DeclareMCAttribute>() ?? throw new InvalidOperationException("Function does not have the DeclareMC attribute");
        }

        public static Type[] Args(Delegate func)
        {
            var types = new List<Type>();

            foreach (var i in func.Method.GetParameters())
            {
                if (i.ParameterType.GetInterface(nameof(IRuntimeArgument)) != null) types.Add(i.ParameterType);
                else throw new InvalidOperationException($"Function {func.Method.Name} is not a valid Minecraft function");
            }

            return [.. types];
        }
    }

    public class DeclareMCReturnAttribute<T> : DeclareMCAttribute where T : IRuntimeArgument
    {
        public DeclareMCReturnAttribute(string name) : base(name)
        {
        }

        public DeclareMCReturnAttribute(string name, string[] macros) : base(name, macros)
        {
        }
    }
}
