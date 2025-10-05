using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.Geode.Values;
using Datapack.Net.Utils;

namespace Amethyst.Geode.Types
{
	public class FunctionTypeSpecifier(FunctionModifiers modifiers, TypeSpecifier returnType, IEnumerable<Parameter> paramters) : TypeSpecifier
	{
		public readonly FunctionModifiers Modifiers = modifiers;
		public readonly TypeSpecifier ReturnType = returnType;
		public readonly Parameter[] Parameters = [.. paramters];
		public override IEnumerable<TypeSpecifier> Subtypes => [ReturnType, .. Parameters.Select(i => i.Type)];
		public override NamespacedID ID => "amethyst:func";
		public override TypeSpecifier BaseClass => this;

		public FunctionTypeSpecifier ApplyGenericWithParams(TypeSpecifier[] args)
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

			var other = new FunctionTypeSpecifier(Modifiers, ReturnType, newArgs);
			return (FunctionTypeSpecifier)ApplyGeneric(other);
		}

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is FunctionTypeSpecifier f
			&& f.Modifiers == Modifiers
			&& f.ReturnType == ReturnType
			&& Parameters.Length == f.Parameters.Length
			&& Parameters.Zip(f.Parameters).All(i => i.First == i.Second);

		// TODO: properly do this
		public override string ToString() => $"{ReturnType}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";
		public string ToString(string name) => $"{ReturnType} {name}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";

		public override object Clone() => new FunctionTypeSpecifier(Modifiers, (TypeSpecifier)ReturnType.Clone(), Parameters.Select(i => new Parameter(i.Modifiers, (TypeSpecifier)i.Type.Clone(), i.Name)));

		public static FunctionTypeSpecifier VoidFunc => new(FunctionModifiers.None, new VoidTypeSpecifier(), []);

		public override LiteralValue DefaultValue => throw new InvalidTypeError(ToString());
	}
}
