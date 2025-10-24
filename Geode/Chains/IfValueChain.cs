using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Values;

namespace Geode.Chains
{
	public class IfValueChain : ExecuteChainConditional
	{
		// Do val != 0
		private IfValueChain(ValueRef val, bool invert = false) : base([val], !invert)
		{
		}

		protected override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute.Conditional cmd)
		{
			var val = processedArgs[0];

			if (val is LiteralValue l)
			{
				if (l.Value.ToString() is "0" or "[]" or "{}" or "" or "\"\"" or "''")
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else if (val is ScoreValue score)
			{
				cmd.Score(score.Target, score.Score, 0);
				return null;
			}

			throw new InvalidTypeError(val.Type.ToString(), "score");
		}

		public static IfValueChain With(ValueRef val, FunctionContext ctx, bool invert = false) => new(ctx.Add(new LoadInsn(val)), invert);
	}
}
