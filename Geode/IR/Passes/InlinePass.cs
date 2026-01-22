using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Passes
{
	public class InlinePass : Pass
	{
		protected override void OnInsn(FunctionContext ctx, Block block, Instruction insn)
		{
			if (insn is CallInsn call && call.Arg<ValueRef>(0).Value is FunctionValue func && func.FuncType.Modifiers.HasFlag(FunctionModifiers.Inline))
			{
				var other = ctx.Compiler.IR.Functions.Find(i => i.Decl.ID == func.ID);

				if (other is null || other.Blocks.Count != 1 || other.Decl.FuncType.Parameters.Length > 0 || other.Decl.FuncType.ReturnType is not VoidType)
				{
					return;
				}

				var (newInsns, newVariables) = other.Blocks.First().Copy("inline_frame");

				foreach (var i in newVariables)
				{
					ctx.RegisterExtraLocal(i);
				}

				for (var i = 0; i < newInsns.Count; i++)
				{
					if (newInsns[i] is ReturnInsn ret)
					{
						if (ret.ReturnType != new VoidType())
						{
							throw new NotImplementedException();
						}

						newInsns = newInsns[..i];
					}
				}

				insn.ReplaceWith([.. newInsns]);
			}
		}
	}
}
