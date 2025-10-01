using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

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

		// public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.Bool;

		protected override ValueRef _Execute(FunctionContext ctx)
		{
			var left = ctx.Add(new LoadInsn(ctx.ImplicitCast(Left.Execute(ctx), PrimitiveTypeSpecifier.Int)));
			var right = ctx.Add(new LoadInsn(ctx.ImplicitCast(Right.Execute(ctx), PrimitiveTypeSpecifier.Int)));

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
