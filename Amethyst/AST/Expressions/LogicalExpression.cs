using Amethyst.Geode;
using Amethyst.Geode.IR;

using static Datapack.Net.Data._1_20_4.Blocks;

namespace Amethyst.AST.Expressions
{
	public enum LogicalOperation
	{
		And,
		Or
	}

	public class LogicalExpression(LocationRange loc, Expression left, LogicalOperation op, Expression right) : Expression(loc)
	{
		public readonly Expression Left = left;
		public readonly LogicalOperation Op = op;
		public readonly Expression Right = right;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.Bool;

		public override ValueRef Execute(FunctionContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
