using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class Kill(FunctionType? type = null) : Intrinsic("minecraft:kill",
		type ?? new FunctionType(FunctionModifiers.None, new VoidType(),
			[new(ParameterModifiers.None, new TargetSelectorType(), "target")]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new Summon(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 1)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			return ctx.Add(new CommandInsn(Raw("kill "), ctx.ImplicitCast(args[0], new TargetSelectorType())));
		}
	}
}