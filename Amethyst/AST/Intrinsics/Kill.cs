using Amethyst.IR.Instructions;
using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Intrinsics
{
	public class Kill(FunctionType? type = null) : Intrinsic("minecraft:kill", type ?? new FunctionType(FunctionModifiers.None, new VoidType(), [new(ParameterModifiers.None, new TargetSelectorType(), "target")]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new Summon(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 1)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			return ctx.Add(new CommandInsn(new LiteralValue(new NBTRawString("kill ")), ctx.ImplicitCast(args[0], new TargetSelectorType())));
		}
	}
}
