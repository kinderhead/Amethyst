using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;

namespace Amethyst.AST.Intrinsics
{
	public class Print(FunctionTypeSpecifier? type = null) : Intrinsic("builtin:print", type)
	{
		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new Print(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			for (var i = 0; i < args.Length; i++)
			{
				if (args[i].Type is ReferenceTypeSpecifier)
				{
					args[i] = ctx.Add(new DereferenceInsn(args[i]));
				}
				else if (args[i].Value is ConditionalValue)
				{
					args[i] = ctx.Add(new LoadInsn(args[i]));
				}
			}

			return ctx.Add(new PrintInsn(args));
		}
	}
}
