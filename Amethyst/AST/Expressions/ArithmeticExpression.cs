using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using Datapack.Net.Function.Commands;

namespace Amethyst.AST.Expressions
{
	public class ArithmeticExpression(LocationRange loc, Expression left, ScoreOperation op, Expression right) : Expression(loc)
	{
		public readonly Expression Left = left;
		public readonly ScoreOperation Op = op;
		public readonly Expression Right = right;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.Int;

		protected override ValueRef _Execute(FunctionContext ctx)
		{
			var left = ctx.Add(new LoadInsn(ctx.ImplicitCast(Left.Execute(ctx), PrimitiveTypeSpecifier.Int)));
			var right = ctx.Add(new LoadInsn(ctx.ImplicitCast(Right.Execute(ctx), PrimitiveTypeSpecifier.Int)));

            return Op switch
			{
				ScoreOperation.Add => ctx.Add(new AddInsn(left, right)),
				ScoreOperation.Sub => ctx.Add(new SubInsn(left, right)),
				ScoreOperation.Mul => ctx.Add(new MulInsn(left, right)),
				ScoreOperation.Div => ctx.Add(new DivInsn(left, right)),
				ScoreOperation.Mod => ctx.Add(new ModInsn(left, right)),
				_ => throw new NotImplementedException(),
			};
		}
	}
}
