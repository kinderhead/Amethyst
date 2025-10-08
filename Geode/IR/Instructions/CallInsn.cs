using Datapack.Net.Data;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class CallInsn(ValueRef func, IEnumerable<ValueRef> args) : Instruction([func, .. args])
	{
		public override string Name => "call";
		public override NBTType?[] ArgTypes => [null, .. FuncType.Parameters.Select(i => i.Type is VarTypeSpecifier ? (NBTType?)null : i.Type.EffectiveType)];
		public override TypeSpecifier ReturnType => FuncType.ReturnType;
		public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Arg<ValueRef>(0).Type;

		public override void Render(RenderContext ctx)
		{
			var func = Arg<ValueRef>(0).Expect<FunctionValue>();
			func.Call(ctx, [.. Arguments[1..].Cast<ValueRef>()]);
			if (ReturnValue.Expect() is LValue ret)
			{
				ret.Store(FunctionContext.GetFunctionReturnValue(ReturnType, -1), ctx);
			}
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			if (ReturnType is VoidTypeSpecifier)
			{
				return new VoidValue();
			}

			return null;
		}
	}
}
