using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGeneratorsKit;

namespace Datapack.Net.SourceGenerator
{
    [Generator]
    public class RuntimeObjectGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var projects = context.SyntaxProvider.ForAttributeWithMetadataName<RuntimeObject?>("Datapack.Net.CubeLib.RuntimeObjectAttribute",
                static (s, _) => true,
                static (ctx, _) =>
                {
                    //if (!Debugger.IsAttached)
                    //{
                    //    Debugger.Launch();
                    //}

                    var cls = (ClassDeclarationSyntax)ctx.TargetNode;

                    foreach (var i in cls.AttributeLists)
                    {
                        foreach (var e in i.Attributes)
                        {
                            if (ctx.SemanticModel.GetSymbolInfo(e).Symbol is not IMethodSymbol attribute) continue;
                            if (attribute.ContainingType.ToDisplayString() == "Datapack.Net.CubeLib.RuntimeObjectAttribute")
                            {
                                if (ctx.SemanticModel.GetDeclaredSymbol(cls) is not INamedTypeSymbol clsSymbol) return null;

                                List<MCFunction> funcs = [];
                                List<RuntimeObjectProperty> props = [];

                                foreach (var sym in clsSymbol.GetMembers())
                                {
                                    if (sym is IMethodSymbol method && method.GetAttributes().Where(i => i.AttributeClass.ToDisplayString().Contains("Datapack.Net.CubeLib.DeclareMC")).Count() != 0 && sym.IsStatic)
                                    {
                                        funcs.Add(Utils.GetMCFunction(method));
                                    }

                                    else if (sym is INamedTypeSymbol propCls && propCls.Name.Contains("Props"))
                                    {
                                        foreach (var propSym in propCls.GetMembers())
                                        {
                                            if (propSym is IPropertySymbol prop && prop.HasAttribute("Datapack.Net.CubeLib.RuntimePropertyAttribute"))
                                            {
                                                var internalName = prop.FindAttribute("Datapack.Net.CubeLib.RuntimePropertyAttribute").ConstructorArguments.FirstOrDefault().Value.ToString();
                                                props.Add(new(prop.Name, internalName, prop.Type.ToDisplayString(), prop.Type.AllInterfaces.Where(i => i.ToDisplayString().Contains("Datapack.Net.CubeLib.IRuntimeProperty")).Count() != 0));
                                            }
                                        }
                                    }
                                }

                                var name = new StringBuilder(clsSymbol.Name);
                                if (clsSymbol.TypeParameters.Length > 0) {
                                    name.Append("<");
                                    foreach (var t in clsSymbol.TypeParameters)
                                    {
                                        name.Append($"{t.Name}, ");
                                    }
                                    name.Length -= 2;
                                    name.Append(">");
                                }

                                return new RuntimeObject(name.ToString(), clsSymbol.ContainingNamespace.ToDisplayString(), !(e.ArgumentList.Arguments.Count == 2 && e.ArgumentList.Arguments[1].Expression.ToString() == "false"), funcs, props);
                            }
                        }
                    }
                    return null;
                }
            ).Where(static m => m is not null);

            context.RegisterSourceOutput(projects, static (spc, source) => Execute(source, spc));
        }

        private static void Execute(RuntimeObject? _obj, SourceProductionContext context)
        {
            if (_obj is not { } obj) return;

            var things = new StringBuilder();

            foreach (var i in obj.Properties)
            {
                var type = i.IsObj ? i.Type : Utils.ProcessRuntimePropertyType(i.Type);
                var postfix = i.IsObj ? "Obj" : "Prop";

                things.AppendLine($"        /// <inheritdoc cref=\"{obj.Namespace}.{obj.Name}.Props.{i.Name}\"/>");
                things.AppendLine($"        public {type} {i.Name} {{ get => new(this.Get{postfix}<{i.Type}>(\"{i.InternalName}\"){(i.IsObj ? ".Pointer" : "")}); set => this.SetProp(\"{i.InternalName}\", value); }}");
            }

            foreach (var i in obj.Methods)
            {
                things.AppendLine(Utils.GenerateWrapper(i, true));
            }

            if (things.Length > 0) things.Append("\n");

            things.Append("        public override (string, Type)[] AllProperties => [");

            if (obj.Properties.Length > 0)
            {
                foreach (var i in obj.Properties)
                {
                    things.Append($"(\"{i.InternalName}\", typeof({i.Type})), ");
                }
                things.Length -= 2;
            }

            things.Append("];");

            if (obj.ImplementCleanup)
            {
                things.Append("\n\n        public override void FreePointers()\n        {\n");

                if (obj.Properties.Length > 0)
                {
                    foreach (var i in obj.Properties)
                    {
                        if (i.IsObj) things.Append($"            ((global::Datapack.Net.CubeLib.Builtins.RuntimePointer<{i.Type}>){i.Name}.Pointer).RemoveOneReference();\n");
                    }
                }

                things.Append("        }");
            }

            string source = $@"/// <auto-generated/>
namespace {obj.Namespace}
{{
    public partial class {obj.Name}
    {{
{things}
    }}
}}";
            context.AddSource($"{obj.Name.Split('<')[0]}.g.cs", source);
        }
    }
}
