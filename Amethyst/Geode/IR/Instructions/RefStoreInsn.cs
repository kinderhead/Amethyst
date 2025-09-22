using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class StoreRefInsn(ValueRef dest, ValueRef src) : Instruction([dest, src])
    {
        public override string Name => "store_ref";
        public override NBTType?[] ArgTypes => [null, null];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();
		public override bool ShouldProcessArgs => false;

        public override void Render(RenderContext ctx)
        {
            var dest = Arg<ValueRef>(0).Expect();
            var src = Arg<ValueRef>(1).Expect();

            if (src.Type is ReferenceTypeSpecifier) ctx.Call("amethyst:core/ref/set-ref", dest, src);
            else if (src is DataTargetValue data) ctx.Call("amethyst:core/ref/set-ref", dest, ReferenceTypeSpecifier.From(data));
            else
            {
                // Stupid minecraft doesn't have a good way to escape strings automatically (so far)
                if (src is LiteralValue l && l.Value is NBTString str) ctx.Call("amethyst:core/ref/set", dest, new LiteralValue($"\"{NBTString.Escape(str.Value)}\""));
                else ctx.Call("amethyst:core/ref/set", dest, src);
            }
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }
}
