using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode;
using Geode.Errors;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class DynCallInsn(ValueRef func) : CallInsn(func, [])
	{
		public override string Name => "dyncall";
		public override NBTType?[] ArgTypes => [NBTType.String];
		public override bool HasSideEffects => true;

		public override void Render(RenderContext ctx)
		{
			var func = Arg<ValueRef>(0).Expect();

			if (func.Type is not FunctionType)
			{
				throw new InvalidTypeError(func.Type.ToString(), "function");
			}

			new StackValue(-1, ctx.Builder.RuntimeID, "func", func.Type).Store(func, ctx);

			if (FuncType.IsMacroFunction)
			{
				ctx.Add(new FunctionCommand("amethyst:core/func/call-macro", ctx.Builder.RuntimeID, "stack[-1]"));
			}
			else
			{
				ctx.Add(new FunctionCommand("amethyst:core/func/call", ctx.Builder.RuntimeID, "stack[-1]"));
			}

			if (ReturnValue.Expect() is LValue ret)
			{
				ret.Store(ctx.Func.GetFunctionReturnValue(ReturnType, -1), ctx);
			}
		}
	}
}
