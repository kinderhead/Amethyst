using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Utils;

namespace Amethyst.Geode.IR.InlineFunctions
{
    [Obsolete("Use ListAddInsn instead")]
    public class ListAdd(NamespacedID id, FunctionTypeSpecifier type) : FunctionValue(id, type)
    {
        public ListAdd() : this("amethyst:add", new FunctionTypeSpecifier(AST.FunctionModifiers.None, new VoidTypeSpecifier(), [
            new(AST.ParameterModifiers.None, new ReferenceTypeSpecifier(new ListTypeSpecifier(new GenericTypeSpecifier("T"))), "this"),
            new(AST.ParameterModifiers.None, new GenericTypeSpecifier("T"), "value")
        ]))
        {
        }

        public override void Call(RenderContext ctx, ValueRef[] args, bool applyGuard = true)
        {
            var list = args[0].Expect<LValue>();
            list.ListAdd(args[1].Expect(), ctx);
        }

        public override FunctionValue CloneWithType(FunctionTypeSpecifier type) => new ListAdd(ID, type);
    }
}
