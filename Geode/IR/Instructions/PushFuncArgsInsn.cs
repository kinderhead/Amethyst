using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
    public class PushFuncArgsInsn(FunctionTypeSpecifier type, IEnumerable<ValueRef> args) : Instruction(args)
    {
        public readonly FunctionTypeSpecifier FuncType = type;

        public override string Name => "push_args";
        public override NBTType?[] ArgTypes => [.. FuncType.Parameters.Select(i => i.Type is VarTypeSpecifier ? (NBTType?)null : i.Type.EffectiveType)];
        public override TypeSpecifier ReturnType => new VoidTypeSpecifier();

        public override void Render(RenderContext ctx)
        {
            var macros = FunctionValue.SetArgsAndGetMacros(ctx, FuncType, [.. Arguments.Cast<ValueRef>()]);

            if (macros is LiteralValue)
            {
                new StackValue(-1, $"macros", PrimitiveTypeSpecifier.Compound).Store(macros, ctx);
            }
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
    }
}
