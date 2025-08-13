using Amethyst.Geode;
using Amethyst.Geode.IR;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
	public enum ComparisonOperator
	{
		Eq,
		Neq,
		Gt,
		Gte,
		Lt,
		Lte
	}

	public class ComparisonExpression(LocationRange loc, Expression left, ComparisonOperator op, Expression right) : Expression(loc)
	{
		public readonly Expression Left = left;
		public readonly ComparisonOperator Op = op;
		public readonly Expression Right = right;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.Bool;

		public override ValueRef Execute(FunctionContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
