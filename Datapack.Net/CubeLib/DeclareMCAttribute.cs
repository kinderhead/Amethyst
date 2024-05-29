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

        public static DeclareMCAttribute Get(Action func)
        {
            return func.Method.GetCustomAttribute<DeclareMCAttribute>() ?? throw new InvalidOperationException("Function does not have the DeclareMC attribute");
        }
    }
}
