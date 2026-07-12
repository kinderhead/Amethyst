using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class Summon(FunctionType? type = null) : Intrinsic("minecraft:summon",
		type ?? new FunctionType(FunctionModifiers.None, EntityType.Dummy,
			[new(ParameterModifiers.None, PrimitiveType.String, "target")]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new Summon(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 1)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			ctx.Add(new CommandInsn(Raw("summon "), ctx.ExplicitCast(args[0], new UnsafeStringType()),
				Raw(" ~ ~ ~ {Tags:[\"__amethyst_summon\"]}")));
			var ret = ctx.Add(new EntityRefInsn(Raw("@e[tag=__amethyst_summon,limit=1]")));
			ctx.Add(new CommandInsn(Raw("tag @e remove __amethyst_summon")));

			return ret;
		}
	}
}