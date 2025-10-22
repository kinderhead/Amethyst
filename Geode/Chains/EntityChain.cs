using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Values;
using System;

namespace Geode.Chains
{
	public class EntityChain(ValueRef target, bool invert = false) : ExecuteChainSubcommand([target], invert)
	{
		public override bool RequireLiteral => true;

		public override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute.Conditional cmd)
        {
            var entity = ((IConstantValue)processedArgs[0]).Value.ToString();
            cmd.Entity(new NamedTarget(entity));
            return null;
        }
	}
}
