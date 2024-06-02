using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using SourceGeneratorsKit;

namespace Datapack.Net.SourceGenerator
{
    public static class Utils
    {
        public static string GenerateWrapper(MCFunction func, string state = "", bool self = false)
        {
            var builder = new StringBuilder();

            builder.Append($"        /// <inheritdoc cref=\"{func.FullyQualifiedName}\"/>\n");
            builder.Append($"        public void {func.Name.Substring(1)}(");

            List<string> args = [];

            for (int i = 0; i < func.Arguments.Length; i++)
            {
                if (i == 0 && self) continue;
                var arg = func.Arguments[i];
                args.Add($"{arg.Item1} {arg.Item2}");
            }

            if (func.Macro) args.Add("System.Collections.Generic.KeyValuePair<string, object>[] macros");
            if (func.ReturnType != "void") args.Add($"{func.ReturnType} ret");

            foreach (var i in args)
            {
                builder.Append(i);
                builder.Append(", ");
            }
            
            if (args.Count > 0) builder.Length -= 2;
            builder.Append(") => ");

            var postfix = func.ReturnType != "void" ? "Ret" : "";

            if (func.Arguments.Length > 0 || self)
            {
                builder.Append($"{state}CallArg{postfix}({func.Name}, ");
                if (func.ReturnType != "void") builder.Append("ret.GetAsArg(), ");
                if (func.Macro) builder.Append("[");
                for (int i = 0; i < func.Arguments.Length; i++)
                {
                    if (i == 0 && self) builder.Append("this, ");
                    else
                    {
                        var arg = func.Arguments[i];
                        builder.Append($"{arg.Item2}, ");
                    }
                }
                builder.Length -= 2;
                if (func.Macro) builder.Append("], macros");
            }
            else if (func.Macro) builder.Append($"{state}Call{postfix}({func.Name}, macros");
            else builder.Append($"{state}Call{postfix}({func.Name}");

            builder.Append(");");

            return builder.ToString();
        }

        public static MCFunction GetMCFunction(IMethodSymbol method)
        {
            List<(string, string)> args = new(method.Parameters.Length);
            foreach (var arg in method.Parameters)
            {
                args.Add((arg.Type.ToDisplayString(), arg.Name));
            }

            var attr = method.GetAttributes().Where(i => i.AttributeClass.ToDisplayString() == "Datapack.Net.CubeLib.DeclareMCAttribute").FirstOrDefault();

            return new(method.Name, method.ToDisplayString(), method.ReturnsVoid ? "void" : method.ReturnType.ToDisplayString(), args, attr.AttributeConstructor.Parameters.Length == 3);
        }
    }
}
