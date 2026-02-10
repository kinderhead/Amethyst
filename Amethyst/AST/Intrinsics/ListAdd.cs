using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Intrinsics
{
	public class ListAdd(FunctionType? type = null) : Intrinsic("amethyst:list/add", type ?? new(FunctionModifiers.None, new VoidType(), [
			new(ParameterModifiers.None, new ReferenceType(new ListType(new GenericType("T"))), "this"),
			new(ParameterModifiers.None, new GenericType("T"), "val")
		]))
	{
		public override IFunctionLike CloneWithType(FunctionType type) => new ListAdd(type);

		public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
		{
			if (args.Length != 2)
			{
				throw new MismatchedArgumentCountError(1, args.Length);
			}

			var list = args[0];
			var val = args[1];

			var type = list.Type is ListType l ? l.Inner : ((ListType)((ReferenceType)list.Type).Inner).Inner;
			return ctx.Add(new ListAddInsn(list, ctx.ImplicitCast(val, type)));
		}
	}
}
