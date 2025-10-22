using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Values;
using System;

namespace Geode.Chains
{
	public class ValueChain : ExecuteChainSubcommand
	{
		public override bool RequireLiteral => false;

        // Do val != 0
        private ValueChain(ValueRef val, bool invert = false) : base([val], !invert)
        {
        }

        public override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute.Conditional cmd)
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

        public static ValueChain With(ValueRef val, FunctionContext ctx, bool invert = false) => new(ctx.Add(new LoadInsn(val)), invert);
    }
}
