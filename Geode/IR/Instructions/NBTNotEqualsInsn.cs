using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
    public class NBTNotEqualsInsn(ValueRef left, ValueRef right) : Instruction([left, right])
    {
        public override NBTType?[] ArgTypes => [null, null];
        public override string Name => "nbt_eq";
        public override TypeSpecifier ReturnType => PrimitiveType.Bool;

        public override void Render(RenderContext ctx)
        {
            var test = ctx.Builder.TempStorage(PrimitiveType.Compound);
            test.Store(Arg<ValueRef>(0).Expect(), ctx);
            ctx.Add(ReturnValue.Expect<LValue>().StoreExecute(false).Run(ctx.WithFaux(ctx => test.Store(Arg<ValueRef>(1).Expect(), ctx)).Single()));
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx) => AreArgsLiteral(out NBTValue left, out NBTValue right)
            ? new LiteralValue(left != right)
            : null;
    }
}