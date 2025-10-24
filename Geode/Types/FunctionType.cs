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
		public readonly FunctionModifiers Modifiers = modifiers;
		public readonly TypeSpecifier ReturnType = returnType;
		public readonly Parameter[] Parameters = [.. parameters];

		public readonly bool IsMacroFunction = parameters.Any(i => i.Modifiers.HasFlag(ParameterModifiers.Macro));
		public readonly MacroValue[] MacroParameters = [.. parameters.Where(i => i.Modifiers.HasFlag(ParameterModifiers.Macro)).Select(i => new MacroValue(i.Name, i.Type))];

		public override IEnumerable<TypeSpecifier> Subtypes => [ReturnType, .. Parameters.Select(i => i.Type)];
		public override NamespacedID ID => "amethyst:func";
		public override TypeSpecifier BaseClass => this;
		public override NBTType EffectiveType => NBTType.String;

		public FunctionType ApplyGenericWithParams(TypeSpecifier[] args)
		{
			var newArgs = new Parameter[Parameters.Length];

			for (var i = 0; i < Parameters.Length; i++)
			{
				if (args.Length > i)
				{
					newArgs[i] = new(Parameters[i].Modifiers, Parameters[i].Type.ApplyGeneric(args[i]), Parameters[i].Name);
				}
				else
				{
					newArgs[i] = Parameters[i];
				}
			}

			var other = new FunctionType(Modifiers, ReturnType, newArgs);
			return (FunctionType)ApplyGeneric(other);
		}

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is FunctionType f
			&& f.Modifiers == Modifiers
			&& f.ReturnType == ReturnType
			&& Parameters.Length == f.Parameters.Length
			&& Parameters.Zip(f.Parameters).All(i => i.First == i.Second);

		// TODO: properly do this
		public override string ToString() => $"{ReturnType}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";
		public string ToString(string name) => $"{ReturnType} {name}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";

		public override object Clone() => new FunctionType(Modifiers, (TypeSpecifier)ReturnType.Clone(), Parameters.Select(i => new Parameter(i.Modifiers, (TypeSpecifier)i.Type.Clone(), i.Name)));

		public static FunctionType VoidFunc => new(FunctionModifiers.None, new VoidType(), []);

		public override LiteralValue DefaultValue => new("", this);
	}
}
