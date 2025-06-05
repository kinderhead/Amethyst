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

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(NBTType.Boolean);

		protected override Value _Execute(FunctionContext ctx)
		{
			var tmp = ctx.AllocTempScore();
			Store(ctx, tmp);
			return tmp;
		}

		protected override void _Store(FunctionContext ctx, MutableValue dest)
		{
			dest.Store(ctx, new LiteralExpression(Location, new NBTBool(false)).Cast(dest.Type).Execute(ctx));
			var exec = new ExecuteWrapper();
			Compare(ctx, exec);
			ctx.Add(new StoreIfSuccessInstruction(Location, exec.Cmd, dest, new LiteralExpression(Location, new NBTBool(true)).Cast(dest.Type).Execute(ctx)));
		}

		protected override void _Compare(FunctionContext ctx, ExecuteWrapper truthy)
		{
			var leftVal = Left.Execute(ctx);
			var rightVal = Right.Execute(ctx);

			if (leftVal is LiteralValue l)
			{
				if (rightVal is LiteralValue r)
				{
					var leftNum = l.ToScoreInt();
					var rightNum = r.ToScoreInt();

					var ret = Op switch
					{
						ComparisonOperator.Eq => leftNum == rightNum,
						ComparisonOperator.Neq => leftNum != rightNum,
						ComparisonOperator.Gt => leftNum > rightNum,
						ComparisonOperator.Gte => leftNum >= rightNum,
						ComparisonOperator.Lt => leftNum < rightNum,
						ComparisonOperator.Lte => leftNum <= rightNum,
						_ => throw new NotImplementedException()
					};

					if (!ret) truthy.NeverExecute = true;

					return;
				}

				LiteralCompare(ctx, rightVal, Op switch
				{
					ComparisonOperator.Eq or ComparisonOperator.Neq => Op,
					ComparisonOperator.Gt => ComparisonOperator.Lt,
					ComparisonOperator.Gte => ComparisonOperator.Lte,
					ComparisonOperator.Lt => ComparisonOperator.Gt,
					ComparisonOperator.Lte => ComparisonOperator.Gte,
					_ => throw new NotImplementedException()
				}, l.ToScoreInt(), truthy);

				return;
			}
			else if (rightVal is LiteralValue r)
			{
				LiteralCompare(ctx, leftVal, Op, r.ToScoreInt(), truthy);
				return;
			}

			var left = leftVal.AsScore(ctx);
			var right = rightVal.AsScore(ctx);

			switch (Op)
			{
				case ComparisonOperator.Eq:
					truthy.Cmd.If.Score(left.Target, left.Score, Comparison.Equal, right.Target, right.Score);
					break;
				case ComparisonOperator.Neq:
					truthy.Cmd.Unless.Score(left.Target, left.Score, Comparison.Equal, right.Target, right.Score);
					break;
				case ComparisonOperator.Gt:
					truthy.Cmd.If.Score(left.Target, left.Score, Comparison.GreaterThan, right.Target, right.Score);
					break;
				case ComparisonOperator.Gte:
					truthy.Cmd.If.Score(left.Target, left.Score, Comparison.GreaterThanOrEqual, right.Target, right.Score);
					break;
				case ComparisonOperator.Lt:
					truthy.Cmd.If.Score(left.Target, left.Score, Comparison.LessThan, right.Target, right.Score);
					break;
				case ComparisonOperator.Lte:
					truthy.Cmd.If.Score(left.Target, left.Score, Comparison.LessThanOrEqual, right.Target, right.Score);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private static void LiteralCompare(FunctionContext ctx, Value leftVal, ComparisonOperator op, int right, ExecuteWrapper truthy)
		{
			if (op == ComparisonOperator.Gt)
			{
				if (right == int.MaxValue)
				{
					truthy.NeverExecute = true;
					return;
				}
				right++;
			}
			else if (op == ComparisonOperator.Lt)
			{
				if (right == int.MinValue)
				{
					truthy.NeverExecute = true;
					return;
				}
				right--;
			}

			var left = leftVal.AsScore(ctx);

			MCRange<int> range = op switch
			{
				ComparisonOperator.Eq or ComparisonOperator.Neq => new(right),
				ComparisonOperator.Gt or ComparisonOperator.Gte => new(right, true),
				ComparisonOperator.Lt or ComparisonOperator.Lte => new(right, false),
				_ => throw new NotImplementedException()
			};

			if (op == ComparisonOperator.Neq)
			{
				truthy.Cmd.Unless.Score(left.Target, left.Score, range);
			}
			else
			{
				truthy.Cmd.If.Score(left.Target, left.Score, range);
			}
		}
	}
}
