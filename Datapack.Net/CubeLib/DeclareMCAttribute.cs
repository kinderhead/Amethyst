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

        public DeclareMCAttribute(string name, bool returns, string[] macros)
        {
            Path = name;
            Returns = returns;
            Macros = macros;
        }

        public DeclareMCAttribute(string name, bool returns = false)
        {
            Path = name;
            Returns = returns;
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
}
