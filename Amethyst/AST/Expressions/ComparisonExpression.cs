using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
	public class ComparisonExpression(LocationRange loc, Expression left, ComparisonOperator op, Expression right) : Expression(loc)
	{
		public readonly Expression Left = left;
		public readonly ComparisonOperator Op = op;
		public readonly Expression Right = right;

		public override void ExecuteChain(ExecuteChain chain, FunctionContext ctx, bool invert = false)
		{
			var left = ctx.Add(new LoadInsn(Left.Execute(ctx, PrimitiveType.Int)));
			var right = ctx.Add(new LoadInsn(Right.Execute(ctx, PrimitiveType.Int)));

			chain.Add(new ScoreChain(left, Op, right, invert));
        }

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var left = ctx.Add(new LoadInsn(Left.Execute(ctx, PrimitiveType.Int)));
			var right = ctx.Add(new LoadInsn(Right.Execute(ctx, PrimitiveType.Int)));

			return Op switch
			{
				ComparisonOperator.Eq => ctx.Add(new EqInsn(left, right)),
				ComparisonOperator.Neq => ctx.Add(new NeqInsn(left, right)),
				ComparisonOperator.Lt => ctx.Add(new LtInsn(left, right)),
				ComparisonOperator.Lte => ctx.Add(new LteInsn(left, right)),
				ComparisonOperator.Gt => ctx.Add(new GtInsn(left, right)),
				ComparisonOperator.Gte => ctx.Add(new GteInsn(left, right)),
				_ => throw new NotImplementedException(),
			};
		}
	}
}
