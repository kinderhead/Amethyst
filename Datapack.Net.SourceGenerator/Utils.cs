using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using SourceGeneratorsKit;

namespace Datapack.Net.SourceGenerator
{
    public static class Utils
    {
        public static string GenerateWrapper(MCFunction func, bool self = false)
        {
            var builder = new StringBuilder();
            var state = "global::Datapack.Net.CubeLib.Project.ActiveProject.";

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

            args.Add("bool macro = false");
            args.Add("int tmp = 0");

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
                builder.Append("[");
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
                builder.Append("]");
                if (func.Macro) builder.Append(", macros");
            } 
            else
            {
                builder.Append($"{state}Call{postfix}({func.Name}");
                if (func.ReturnType != "void") builder.Append(", ret.GetAsArg()");
                if (func.Macro) builder.Append(", macros");
            }

            builder.Append(", macro");

            if (func.Macro) builder.Append(", tmp");
            builder.Append($");\n        public Datapack.Net.Function.MCFunction {func.Name.Substring(1)}_Function() => {state}FindFunction({func.Name}) ?? throw new Exception(\"Project not built yet\");");

            return builder.ToString();
        }

        public static MCFunction GetMCFunction(IMethodSymbol method)
        {
            List<(string, string)> args = new(method.Parameters.Length);
            foreach (var arg in method.Parameters)
            {
                args.Add((arg.Type.ToDisplayString(), arg.Name));
            }

            var attr = method.GetAttributes().Where(i => i.AttributeClass.ToDisplayString().Contains("Datapack.Net.CubeLib.DeclareMC")).FirstOrDefault();

            return new(method.Name, method.ToDisplayString(), attr.AttributeClass.IsGenericType ? attr.AttributeClass.TypeArguments.FirstOrDefault().ToDisplayString() : "void", args, attr.AttributeConstructor.Parameters.Length == 2);
        }

        public static string ProcessRuntimePropertyType(string type)
        {
            if (type == "Datapack.Net.Data.NBTString") return "Datapack.Net.CubeLib.StringRuntimeProperty";
            if (type == "Datapack.Net.Data.NBTInt") return "Datapack.Net.CubeLib.IntRuntimeProperty";
            if (type == "Datapack.Net.Data.NBTLong") return "Datapack.Net.CubeLib.LongRuntimeProperty";
            if (type == "Datapack.Net.Data.NBTShort") return "Datapack.Net.CubeLib.ShortRuntimeProperty";
            if (type == "Datapack.Net.Data.NBTFloat") return "Datapack.Net.CubeLib.FloatRuntimeProperty";
            if (type == "Datapack.Net.Data.NBTDouble") return "Datapack.Net.CubeLib.DoubleRuntimeProperty";
            if (type == "Datapack.Net.Data.NBTByte") return "Datapack.Net.CubeLib.ByteRuntimeProperty";
            if (type == "Datapack.Net.Data.NBTBool") return "Datapack.Net.CubeLib.BoolRuntimeProperty";

            return $"global::Datapack.Net.CubeLib.RuntimeProperty<{type}>";
        }
    }
}
