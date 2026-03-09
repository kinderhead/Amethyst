using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using System;

namespace Amethyst.AST.Intrinsics
{
    public class ListRemoveAt(FunctionType? type = null) : Intrinsic("amethyst:list/remove_at", type ?? new(FunctionModifiers.None, new VoidType(), [
            new(ParameterModifiers.None, new ReferenceType(new ListType(new GenericType("T"))), "this"),
            new(ParameterModifiers.None, PrimitiveType.Int, "index")
        ]))
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new ListRemoveAt(type);

        public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
        {
            if (args.Length != 2)
            {
                throw new MismatchedArgumentCountError(2, args.Length);
            }

            var list = args[0];
            var index = args[1];

            return ctx.Add(new ListRemoveInsn(list, ctx.ImplicitCast(index, PrimitiveType.Int)));
        }
    }
}
