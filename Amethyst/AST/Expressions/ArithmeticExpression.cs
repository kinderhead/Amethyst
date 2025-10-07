using Datapack.Net.Function.Commands;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
	public class ArithmeticExpression(LocationRange loc, Expression left, ScoreOperation op, Expression right) : Expression(loc)
	{
		public readonly Expression Left = left;
		public readonly ScoreOperation Op = op;
		public readonly Expression Right = right;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var left = ctx.Add(new LoadInsn(Left.Execute(ctx, PrimitiveTypeSpecifier.Int)));
			var right = ctx.Add(new LoadInsn(Right.Execute(ctx, PrimitiveTypeSpecifier.Int)));

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
