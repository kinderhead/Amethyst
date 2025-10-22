using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Values;
using System;

namespace Geode.Chains
{
    public class AtChain(ValueRef target) : ExecuteChainSubcommand([target])
    {
        public override bool RequireLiteral => true;
        public override bool Forks => true;

        public override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute cmd)
        {
            var entity = ((IConstantValue)processedArgs[0]).Value.ToString();
            cmd.At(new NamedTarget(entity));
            return null;
        }
    }
}
