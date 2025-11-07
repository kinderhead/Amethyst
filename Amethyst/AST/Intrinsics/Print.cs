using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Intrinsics
{
	public class Print(FunctionType? type = null) : Intrinsic("builtin:print", type)
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new Print(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			for (var i = 0; i < args.Length; i++)
			{
				if (args[i].Type is ReferenceType)
				{
					args[i] = ctx.Add(new DereferenceInsn(args[i]));
				}
			}

			return ctx.Add(new PrintInsn(args));
		}
	}
}
