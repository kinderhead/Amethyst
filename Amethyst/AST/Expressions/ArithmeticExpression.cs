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
			ThrowIfInvalid(ctx);
			if (IsLiteral() is LiteralValue l) return l;

			var tmp = ctx.AllocTempScore();
			Store(ctx, tmp);
			return tmp;
		}

		protected override void _Store(FunctionContext ctx, MutableValue dest)
		{
			ThrowIfInvalid(ctx);

			if (IsLiteral() is LiteralValue l)
			{
				dest.Store(ctx, l);
				return;
			}

			if (dest is not ScoreValue sval)
			{
				dest.Store(ctx, Execute(ctx));
				return;
			}
			
			

			Left.Store(ctx, sval);
			ctx.Add(new ScoreOperationInstruction(Location, sval, Op, Right.Execute(ctx).AsScore(ctx)));
		}

		private void ThrowIfInvalid(FunctionContext ctx)
		{
			var lt = Left.ComputeType(ctx);
			var rt = Right.ComputeType(ctx);
			if (!lt.Operable) throw new InvalidTypeError(Location, lt.ToString());
			if (!rt.Operable) throw new InvalidTypeError(Location, rt.ToString());
		}

		private LiteralValue? IsLiteral()
		{
			if (Left is LiteralExpression l && Right is LiteralExpression r)
			{
				var left = int.Parse(l.Value.ToString()); // This is certainly one way to do this
				var right = int.Parse(r.Value.ToString());
				var ret = Op switch
				{
					ScoreOperation.Add => left + right,
					ScoreOperation.Sub => left - right,
					ScoreOperation.Mul => left * right,
					ScoreOperation.Div => left / right,
					ScoreOperation.Mod => left % right,
					_ => throw new NotImplementedException(),
				};
				return new LiteralValue(new NBTInt(ret));
			}

			return null;
		}
	}
}
