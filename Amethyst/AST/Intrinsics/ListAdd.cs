using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

namespace Amethyst.AST.Intrinsics
{
    public class ListAdd(FunctionTypeSpecifier? type = null) : Intrinsic("amethyst:add", type ?? new(FunctionModifiers.None, new VoidTypeSpecifier(), [
            new(ParameterModifiers.None, new ReferenceTypeSpecifier(new ListTypeSpecifier(new GenericTypeSpecifier("T"))), "this"),
            new(ParameterModifiers.None, new GenericTypeSpecifier("T"), "value")
        ]))
    {
        public override IFunctionLike CloneWithType(FunctionTypeSpecifier type) => new ListAdd(type);

        public override ValueRef Execute(FunctionContext ctx, params ValueRef[] args)
        {
            var list = args[0];
            var val = args[1];

            if (list.Type is ListTypeSpecifier l) return ctx.Add(new ListAddInsn(list, ctx.ImplicitCast(val, l.Inner)));
            else throw new NotImplementedException();
        }
    }
}
