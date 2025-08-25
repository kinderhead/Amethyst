using Amethyst.AST.Statements;
using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Datapack.Net.Utils;

namespace Amethyst.AST
{
	[Flags]
	public enum FunctionModifiers
	{
		None = 0,
		NoStack = 1,
		Inline = 2
	}

	public class FunctionNode(LocationRange loc, List<NamespacedID> tags, FunctionModifiers modifiers, AbstractTypeSpecifier ret, NamespacedID id, List<AbstractParameter> parameters, BlockNode body) : Node(loc), IRootChild
	{
		public readonly List<NamespacedID> Tags = tags;
		public readonly FunctionModifiers Modifiers = modifiers;
		public readonly AbstractTypeSpecifier ReturnType = ret;
		public readonly NamespacedID ID = id;
		public readonly List<AbstractParameter> Parameters = parameters;
		public readonly BlockNode Body = body;

		private FunctionTypeSpecifier? funcType = null;
		public FunctionTypeSpecifier GetFunctionType(Compiler ctx) => funcType ??= new(Modifiers, ReturnType.Resolve(ctx), Parameters.Select(i => new Parameter(i.Modifiers, i.Type.Resolve(ctx), i.Name)));

		public bool Compile(Compiler compiler, out FunctionContext? ctx)
		{
			ctx = null;

			var funcType = GetFunctionType(compiler);

			if (Modifiers.HasFlag(FunctionModifiers.Inline))
			{
				if (funcType.Parameters.Any(i => i.Modifiers.HasFlag(ParameterModifiers.Macro))) throw new ModifierError("inline functions cannot have macro arguments");
				return false;
			}

			ctx = new FunctionContext(compiler, (StaticFunctionValue)compiler.Symbols[ID].Value, Tags);

			if (!Body.Statements.Any() || Body.Statements.Last() is not ReturnStatement)
			{
				if (funcType.ReturnType is VoidTypeSpecifier) Body.Add(new ReturnStatement(Location, null));
				else throw new MissingReturnError();
			}

			if (!Body.CompileWithErrorChecking(ctx)) return false;

			ctx.Finish();

			return true;
		}

		public void Process(Compiler ctx)
		{
			if (ctx.Symbols.TryGetValue(ID, out var sym)) throw new RedefinedSymbolError(ID.ToString(), sym.Location);
			ctx.Symbols[ID] = new(ID, Location, new StaticFunctionValue(ID, GetFunctionType(ctx)), this);
		}
	}

	[Flags]
	public enum ParameterModifiers
	{
		None = 0,
		Macro = 1
	}

	public readonly record struct AbstractParameter(ParameterModifiers Modifiers, AbstractTypeSpecifier Type, string Name);
	public readonly record struct Parameter(ParameterModifiers Modifiers, TypeSpecifier Type, string Name);
}
