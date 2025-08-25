using Datapack.Net.Data;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.IR.Instructions
{
    public class CallInsn(ValueRef func, IEnumerable<ValueRef> args) : Instruction([func, .. args])
    {
        public override string Name => "call";
        public override NBTType?[] ArgTypes => [null, .. FuncType.Parameters.Select(i => i.Type.EffectiveType)];
        public override TypeSpecifier ReturnType => FuncType.ReturnType;
        public FunctionTypeSpecifier FuncType => Arg<ValueRef>(0).Expect<StaticFunctionValue>().FuncType;

        public override void Render(RenderContext ctx)
        {
            var func = Arg<ValueRef>(0).Expect<StaticFunctionValue>();

            if (Arguments.Length > 1)
            {
                ctx.StoreCompound(new(GeodeBuilder.RuntimeID, "stack[-1].args", PrimitiveTypeSpecifier.Compound),
                    new(func.FuncType.Parameters.Zip(Arguments[1..]).Select(i => new KeyValuePair<string, ValueRef>(i.First.Name, ((ValueRef)i.Second).Expect())))
                );
            }

            ctx.Add(new FunctionCommand(new(func.ID, true)));

            if (ReturnValue.Expect() is LValue ret) ret.Store(FunctionContext.GetFunctionReturnValue(ReturnType, -1), ctx);
        }

        protected override Value? ComputeReturnValue()
        {
            if (ReturnType is VoidTypeSpecifier) return new VoidValue();
            return null;
        }
    }
}
