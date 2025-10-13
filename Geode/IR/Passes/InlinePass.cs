using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Passes
{
	public class InlinePass : Pass
	{
		protected override void OnInsn(FunctionContext ctx, Block block, Instruction insn)
		{
			return; // WIP

			if (insn is CallInsn call && call.Arg<ValueRef>(0).Value is FunctionValue func && func.FuncType.Modifiers.HasFlag(FunctionModifiers.Inline))
			{
				var other = ctx.Compiler.IR.Functions.Find(i => i.Decl.ID == func.ID);

				if (other is null || other.Blocks.Count != 1)
				{
					return;
				}

				var newInsns = other.Blocks.First().Copy();

				for (var i = 0; i < newInsns.Count; i++)
				{
					if (newInsns[i] is ReturnInsn ret)
					{
						if (ret.ReturnType != new VoidTypeSpecifier())
						{
							throw new NotImplementedException();
						}

						newInsns.RemoveAt(i);
					}
				}

				insn.ReplaceWith([.. newInsns]);
			}
		}
	}
}
