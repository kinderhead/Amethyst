using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Values;

namespace Geode.Chains
{
	public class IfEntityChain(ValueRef target, bool invert = false) : ExecuteChainConditional([target], invert)
	{
		public override bool RequireLiteral => true;

		protected override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute.Conditional cmd)
		{
			var entity = ((IConstantValue)processedArgs[0]).Value.ToString();
			cmd.Entity(new NamedTarget(entity));
			return null;
		}
	}
}
