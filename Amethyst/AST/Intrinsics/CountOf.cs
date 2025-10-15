using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions.Utils;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Intrinsics
{
	public class CountOf(FunctionType? type = null) : Intrinsic("builtin:count_of", type ?? new FunctionType(FunctionModifiers.None, PrimitiveType.Int, [new(ParameterModifiers.None, PrimitiveType.String, "id")]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new CountOf(type);

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
