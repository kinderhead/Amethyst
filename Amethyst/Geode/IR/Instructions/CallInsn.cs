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
            Value? processedMacros = null;

            if (Arguments.Length > 1)
            {
                var args = new Dictionary<string, ValueRef>();
                var macros = new Dictionary<string, ValueRef>();

                foreach (var (param, val) in FuncType.Parameters.Zip(Arguments[1..].Select(i => (ValueRef)i)))
                {
                    if (param.Modifiers.HasFlag(AST.ParameterModifiers.Macro)) macros.Add(param.Name, val);
                    else args.Add(param.Name, val);
                }

                if (args.Count != 0) ctx.StoreCompound(new(GeodeBuilder.RuntimeID, "stack[-1].args", PrimitiveTypeSpecifier.Compound), args);
                if (macros.Count != 0) processedMacros = ctx.StoreCompoundOrReturnConstant(new(GeodeBuilder.RuntimeID, "stack[-1].macros", PrimitiveTypeSpecifier.Compound), macros);
            }

            if (processedMacros is StorageValue s) ctx.Add(new FunctionCommand(func.ID, s.Storage, s.Path));
            else if (processedMacros is LiteralValue l) ctx.Add(new FunctionCommand(func.ID, (NBTCompound)l.Value));
            else ctx.Add(new FunctionCommand(func.ID));

            if (ReturnValue.Expect() is LValue ret) ret.Store(FunctionContext.GetFunctionReturnValue(ReturnType, -1), ctx);
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx)
        {
            if (ReturnType is VoidTypeSpecifier) return new VoidValue();
            return null;
        }
    }
}
