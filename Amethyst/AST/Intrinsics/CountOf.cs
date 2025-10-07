using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions.Utils;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Intrinsics
{
	public class CountOf(FunctionTypeSpecifier? type = null) : Intrinsic("builtin:count_of", type ?? new FunctionTypeSpecifier(FunctionModifiers.None, PrimitiveTypeSpecifier.Int, [new(ParameterModifiers.None, PrimitiveTypeSpecifier.String, "id")]))
	{
		public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new CountOf(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 1)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			var arg = args.First();
			if (arg.Expect<LiteralValue>().Value is not NBTString id)
			{
				throw new InvalidTypeError(arg.Type.ToString(), "constant string");
			}

			return ctx.Add(new CountOfInsn(id.Value));
		}
	}
}
