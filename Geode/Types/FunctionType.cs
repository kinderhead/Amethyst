using System.Collections.Immutable;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Values;

namespace Geode.Types
{
    [Flags]
    public enum ParameterModifiers
    {
        None = 0,
        Macro = 1
    }

    [Flags]
    public enum FunctionModifiers
    {
        None = 0,
        Inline = 1,
        Virtual = 2
    }

    public readonly record struct Parameter(ParameterModifiers Modifiers, TypeSpecifier Type, string Name);

    public class FunctionType(FunctionModifiers modifiers, TypeSpecifier returnType, IEnumerable<Parameter> parameters) : TypeSpecifier
    {
        public readonly bool IsMacroFunction = parameters.Any(i => i.Modifiers.HasFlag(ParameterModifiers.Macro));

        public readonly ImmutableArray<MacroValue> MacroParameters =
        [
            .. parameters.Index().Where(i => i.Item.Modifiers.HasFlag(ParameterModifiers.Macro))
                         .Select(i => new MacroValue($"arg{i.Index}", i.Item.Type))
        ];

        public readonly FunctionModifiers Modifiers = modifiers;
        public readonly TypeArray ParameterTypes = new([.. parameters.Select(i => i.Type)]);
        public readonly ImmutableArray<Parameter> Parameters = [.. parameters];
        public readonly TypeSpecifier ReturnType = returnType;

        public override IEnumerable<TypeSpecifier> Subtypes => [.. Parameters.Select(i => i.Type), ReturnType];
        public override NamespacedID ID => "amethyst:func";
        public override TypeSpecifier BaseClass => this;
        public override NBTType EffectiveType => NBTType.String;

        public static FunctionType VoidFunc => new(FunctionModifiers.None, new VoidType(), []);

        public override LiteralValue DefaultValue => new("", this);

        public FunctionType ApplyGenericWithParams(TypeSpecifier[] args)
        {
            var newArgs = new Parameter[Parameters.Length];

            for (var i = 0; i < Parameters.Length; i++)
            {
                if (args.Length > i)
                {
                    newArgs[i] = new(Parameters[i].Modifiers, Parameters[i].Type.ApplyGeneric(args[i]), Parameters[i].Name);
                }
                else newArgs[i] = Parameters[i];
            }

            var other = new FunctionType(Modifiers, ReturnType, newArgs);
            return (FunctionType)ApplyGeneric(other);
        }

        protected override bool EqualsImpl(TypeSpecifier obj) => obj is FunctionType f
                                                                 && f.Modifiers == Modifiers
                                                                 && f.ReturnType == ReturnType
                                                                 && Parameters.Length == f.Parameters.Length
                                                                 && Parameters.Zip(f.Parameters)
                                                                              .All(i => i.First.Type == i.Second.Type && i.First.Modifiers == i.Second.Modifiers);

        public override string ToString() => $"{ReturnType}{ParamsToString()}";
        public string ToString(string name) => $"{ReturnType} {name}{ParamsToString()}";

        public string ParamsToString() => $"({string.Join(", ", Parameters.Select(p => {
            return p.Modifiers switch
            {
                ParameterModifiers.None => $"{p.Type}",
                ParameterModifiers.Macro => $"macro {p.Type}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }))})";

        public override object Clone() => new FunctionType(Modifiers, (TypeSpecifier)ReturnType.Clone(), Parameters.Select(i => i with { Type = (TypeSpecifier)i.Type.Clone() }));
    }
}