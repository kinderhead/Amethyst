using Amethyst.Geode;
using Amethyst.Geode.IR;

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

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected) => throw new NotImplementedException();
	}
}
