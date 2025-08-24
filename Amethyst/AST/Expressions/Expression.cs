using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
	public abstract class Expression(LocationRange loc) : Node(loc)
	{
		public virtual Expression Cast(TypeSpecifier type) => new CastExpression(Location, type, this);

		public abstract TypeSpecifier ComputeType(FunctionContext ctx);
		public abstract ValueRef Execute(FunctionContext ctx);

		public virtual ValueRef ExecuteWithoutLoad(FunctionContext ctx) => Execute(ctx);
	}

	public class LiteralExpression(LocationRange loc, NBTValue val) : Expression(loc)
	{
		public readonly NBTValue Value = val;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(Value.Type);
		public override ValueRef Execute(FunctionContext ctx) => new LiteralValue(Value);
	}

	public class VariableExpression(LocationRange loc, string name) : Expression(loc)
	{
		public readonly string Name = name;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => ctx.GetVariable(Name).Type;
		public override ValueRef Execute(FunctionContext ctx) => ctx.Add(new LoadInsn(ctx.GetVariable(Name)));
		public override ValueRef ExecuteWithoutLoad(FunctionContext ctx) => ctx.GetVariable(Name);
	}
}
