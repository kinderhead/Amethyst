using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode;
using Geode.Values;
using System;

namespace Amethyst.IR.Chains
{
	public class IfReferenceExists(ValueRef ptr, bool invert) : ExecuteChainConditional([ptr], invert)
	{
        public override bool RequireLiteral => true;

        protected override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute.Conditional cmd)
        {
            cmd.Data(new RawDataTarget(((IConstantValue)processedArgs[0]).Value.ToString()));
            return null;
        }
	}
}
