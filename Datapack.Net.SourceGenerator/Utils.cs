using System;
using System.Text;

namespace Datapack.Net.SourceGenerator
{
    public static class Utils
    {
        public static string GenerateWrapper(MCFunction func, string state = "", bool self = false)
        {
            var builder = new StringBuilder();

            //builder.Append($"        //<inheritdoc cref=\"{method.GetDocumentationCommentId()}\"/>\n");
            builder.Append($"        public void {func.Name.Substring(1)}(");

            for (int i = 0; i < func.Arguments.Length; i++)
            {
                if (i == 0 && self) continue;
                var arg = func.Arguments[i];
                builder.Append($"{arg.Item1} {arg.Item2}, ");
            }

            if (func.Arguments.Length > (self ? 1 : 0)) builder.Length -= 2;
            builder.Append(") => ");

            if (func.Arguments.Length > 0 || self)
            {
                builder.Append($"{state}CallArg({func.Name}, ");
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
            }
            else
            {
                builder.Append($"{state}Call({func.Name}");
            }

            builder.Append(");");

            return builder.ToString();
        }
    }
}
