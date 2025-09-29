using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class Print(FunctionTypeSpecifier? type = null) : Intrinsic("builtin:print", type)
	{
		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new Print(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			return ctx.Add(new PrintInsn(args));
		}
	}
}
