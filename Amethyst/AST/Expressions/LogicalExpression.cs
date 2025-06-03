using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(NBTType.Boolean);

		protected override Value _Execute(FunctionContext ctx)
		{
			var tmp = ctx.AllocTempScore();
			Store(ctx, tmp);
			return tmp;
		}

		protected override void _Store(FunctionContext ctx, MutableValue dest)
		{
			throw new NotImplementedException();
		}

		protected override void _Compare(FunctionContext ctx, ExecuteWrapper truthy)
		{
			if (Op == LogicalOperation.And)
			{
				Left.Compare(ctx, truthy);
				Right.Compare(ctx, truthy);
			}
			else
			{
				var left = new ExecuteWrapper();
				var right = new ExecuteWrapper();
				Left.Compare(ctx, left);
				Right.Compare(ctx, right);

				if (left.NeverExecute && right.NeverExecute)
				{
					truthy.NeverExecute = true;
					return;
				}

				// Make sure to do this after potential function calls during Compare
				var tmp = ctx.AllocTempScore();
				tmp.Store(ctx, new LiteralValue(new NBTInt(0)));
				ctx.Add(new StoreIfSuccessInstruction(Location, left.Cmd, tmp, new LiteralValue(new NBTInt(1))));
				ctx.Add(new StoreIfSuccessInstruction(Location, right.Cmd, tmp, new LiteralValue(new NBTInt(1))));
				truthy.Cmd.Unless.Score(tmp.Target, tmp.Score, 0);
			}
		}
	}
}
