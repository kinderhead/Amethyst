using Amethyst.Errors;
using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
    public class ResolveWeakRefInsn(ValueRef val) : Instruction([val])
    {
        public override string Name => "wref";
        public override NBTType?[] ArgTypes => [NBTType.String];
        public override TypeSpecifier ReturnType => new ReferenceType(((WeakReferenceType)Arg<ValueRef>(0).Type).Inner);

        public override void Render(RenderContext ctx)
        {
            var val = (NBTString)Arg<ValueRef>(0).Expect<LiteralValue>().Value;
            ctx.Call(ctx.Func.GetGlobalOrThrow<IMinimalFunction>("amethyst:core/ref/get-stack-ref").Get(new([new UnsafeStringType(), PrimitiveType.Int])).ID,
                new LiteralValue(val.Value.Split("[-1].")[^1]), new LiteralValue(-1));
            ReturnValue.Expect<LValue>().Store(ctx.Func.GetFunctionReturnValue(ReturnType, -1), ctx);
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx)
        {
            var val = Arg<ValueRef>(0).Expect();
            if (val is not LiteralValue { Value: NBTString ptr }) throw new WeakReferenceError();

            if (ptr.Value.Contains("stack[-1].")) return null;

            if (ptr.Value.Contains("stack[-2].")) throw new NotImplementedException();

            return new LiteralValue(ptr, ReturnType);
        }
    }
}