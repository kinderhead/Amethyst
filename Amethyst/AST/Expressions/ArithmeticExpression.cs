using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public class ArithmeticExpression(LocationRange loc, Expression left, ScoreOperation op, Expression right) : Expression(loc)
	{
		public readonly Expression Left = left;
		public readonly ScoreOperation Op = op;
		public readonly Expression Right = right;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(NBTType.Int);

		protected override Value _Execute(FunctionContext ctx)
		{
			var tmp = ctx.AllocTempScore();
			Store(ctx, tmp);
			return tmp;
		}

		protected override void _Store(FunctionContext ctx, MutableValue dest)
		{
			if (dest is not ScoreValue sval)
			{
				dest.Store(ctx, Execute(ctx));
				return;
			}
			
			var lt = Left.ComputeType(ctx);
			var rt = Right.ComputeType(ctx);
			if (!NBTValue.IsOperableType(lt.Type)) throw new InvalidTypeError(Location, lt.ToString());
			if (!NBTValue.IsOperableType(rt.Type)) throw new InvalidTypeError(Location, rt.ToString());

			Left.Store(ctx, sval);
			ctx.Add(new ScoreOperationInstruction(Location, sval, Op, Right.Execute(ctx).AsScore(ctx)));
		}
	}
}
