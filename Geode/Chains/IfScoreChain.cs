using Datapack.Net.Function.Commands;
using Geode.IR.Instructions;

namespace Geode.Chains
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

	public class IfScoreChain : ExecuteChainConditional
	{
		public readonly ComparisonOperator Op;

		public IfScoreChain(ValueRef left, ComparisonOperator op, ValueRef right, bool invert = false) : base([left, right], invert)
		{
			Op = op;

			if (Op == ComparisonOperator.Neq)
			{
				// Could probably do some fancy one-line logic in the constructor
				Invert = !Invert;
			}
		}

		protected override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute.Conditional cmd)
		{
			var left = new ValueRef(processedArgs[0]);
			var right = new ValueRef(processedArgs[1]);

			ComparisonInsn insn = Op switch
			{
				ComparisonOperator.Eq => new EqInsn(left, right),
				ComparisonOperator.Neq => new NeqInsn(left, right),
				ComparisonOperator.Lt => new LtInsn(left, right),
				ComparisonOperator.Lte => new LteInsn(left, right),
				ComparisonOperator.Gt => new GtInsn(left, right),
				ComparisonOperator.Gte => new GteInsn(left, right),
				_ => throw new NotImplementedException()
			};

			insn.Resolve(ctx.Func);

			// if (insn.MarkedForRemoval)
			// {
			// 	throw new NotImplementedException();
			// }

			var realLeft = left.Expect().AsScore(ctx);
			var realRight = right.Expect().AsScore(ctx);

			cmd.Score(realLeft.Target, realLeft.Score, insn.Op, realRight.Target, realRight.Score);

			return null;
		}
	}
}
