using Amethyst.Errors;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	public class ResolveWeakRefInsn(ValueRef val) : Instruction([val])
	{
		public override string Name => "wref";
		public override NBTType?[] ArgTypes => [NBTType.String];
		public override TypeSpecifier ReturnType => new ReferenceTypeSpecifier(((WeakReferenceTypeSpecifier)Arg<ValueRef>(0).Type).Inner);

		public override void Render(RenderContext ctx)
		{
			var val = (NBTString)Arg<ValueRef>(0).Expect<LiteralValue>().Value;
			ctx.Call("amethyst:core/ref/get-stack-ref", new LiteralValue(val.Value.Split("[-1].")[^1]), new LiteralValue(-1));
			ReturnValue.Expect<LValue>().Store(FunctionContext.GetFunctionReturnValue(ReturnType, -1), ctx);
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();
			if (val is not LiteralValue l || l.Value is not NBTString ptr)
			{
				throw new WeakReferenceError();
			}
			else if (ptr.Value.Contains("stack[-1]."))
			{
				return null;
			}
			else if (ptr.Value.Contains("stack[-2]."))
			{
				throw new NotImplementedException();
			}
			else
			{
				return new LiteralValue(ptr, ReturnType);
			}
		}
	}
}
