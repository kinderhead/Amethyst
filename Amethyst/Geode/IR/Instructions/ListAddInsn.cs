using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class ListAddInsn(ValueRef list, ValueRef val) : Instruction([list, val])
    {
        public override string Name => "add";
        public override NBTType?[] ArgTypes => [NBTType.List, null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            var list = Arg<ValueRef>(0).Expect();

            if (list.Type is ListTypeSpecifier && list is LValue l) l.ListAdd(Arg<ValueRef>(1).Expect(), ctx);
            else throw new NotImplementedException();
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }
}
