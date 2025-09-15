using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
	public abstract class Expression(LocationRange loc) : Node(loc)
	{
		public abstract TypeSpecifier ComputeType(FunctionContext ctx);
		
		public ValueRef Execute(FunctionContext ctx)
		{
			ValueRef? ret = null;
			if (!ctx.Compiler.WrapError(Location, () => ret = _Execute(ctx))) throw new EmptyAmethystError();
			return ret!;
		}

        protected abstract ValueRef _Execute(FunctionContext ctx);
    }

	public class LiteralExpression(LocationRange loc, NBTValue val) : Expression(loc)
	{
		public readonly NBTValue Value = val;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(Value.Type);
		protected override ValueRef _Execute(FunctionContext ctx) => new LiteralValue(Value);
	}

	public class VariableExpression(LocationRange loc, string name) : Expression(loc)
	{
		public readonly string Name = name;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => ctx.GetVariable(Name).Type;
		protected override ValueRef _Execute(FunctionContext ctx) => ctx.GetVariable(Name);
	}
}
