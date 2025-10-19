using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
    public class PushFuncArgsInsn(FunctionType type, IEnumerable<ValueRef> args) : Instruction(args)
    {
        public readonly FunctionType FuncType = type;

        public override string Name => "push_args";
        public override NBTType?[] ArgTypes => [.. FuncType.Parameters.Select(i => i.Type is VarType ? (NBTType?)null : i.Type.EffectiveType)];
        public override TypeSpecifier ReturnType => new VoidType();

        public override void Render(RenderContext ctx)
        {
            var macros = FunctionValue.SetArgsAndGetMacros(ctx, FuncType, [.. Arguments.Cast<ValueRef>()]);

            if (macros is LiteralValue)
            {
                new StackValue(-1, ctx.Builder.RuntimeID, $"macros", PrimitiveType.Compound).Store(macros, ctx);
            }
        }

        protected override IValue? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }
}
