using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
    public class DynCallInsn(ValueRef func) : CallInsn(func, [])
    {
        public override string Name => "dyncall";
        public override NBTType?[] ArgTypes => [NBTType.String];

        public override void Render(RenderContext ctx)
        {
            var func = Arg<ValueRef>(0).Expect();

            new StackValue(-1, "func", func.Type).Store(func, ctx);

            if (FuncType.IsMacroFunction)
            {
                ctx.Add(new FunctionCommand("amethyst:core/func/call-macro", GeodeBuilder.RuntimeID, "stack[-1]"));
            }
            else
            {
                ctx.Add(new FunctionCommand("amethyst:core/func/call", GeodeBuilder.RuntimeID, "stack[-1]"));
            }

            if (ReturnValue.Expect() is LValue ret)
            {
                ret.Store(FunctionContext.GetFunctionReturnValue(ReturnType, -1), ctx);
            }
        }
    }
}
