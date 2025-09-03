using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Datapack.Net.Function.Commands;

namespace Amethyst.AST.Expressions
{
	public class ArithmeticExpression(LocationRange loc, Expression left, ScoreOperation op, Expression right) : Expression(loc)
	{
		public readonly Expression Left = left;
		public readonly ScoreOperation Op = op;
		public readonly Expression Right = right;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.Int;

		public override ValueRef Execute(FunctionContext ctx)
		{
			var lType = Left.ComputeType(ctx);
			var rType = Right.ComputeType(ctx);
			if (lType.EffectiveType != Datapack.Net.Data.NBTType.Int) throw new InvalidTypeError(lType.ToString(), "int");
			if (rType.EffectiveType != Datapack.Net.Data.NBTType.Int) throw new InvalidTypeError(rType.ToString(), "int");

			var left = Left.Execute(ctx);
			var right = Right.Execute(ctx);

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
