using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class StoreRefInsn(ValueRef dest, ValueRef src) : Instruction([dest, src])
	{
		public override string Name => "store_ref";
		public override NBTType?[] ArgTypes => [NBTType.String, null];
		public override TypeSpecifier ReturnType => new VoidType();

		public override void Render(RenderContext ctx)
		{
			var dest = Arg<ValueRef>(0).Expect();
			var src = Arg<ValueRef>(1).Expect();

			if (dest is IConstantValue c && c.Value is NBTString ptr)
			{
				new RawDataTargetValue(ptr.Value, PrimitiveType.Compound).Store(src, ctx);
			}
			else if (src is DataTargetValue nbt)
			{
				ctx.Call("amethyst:core/ref/set-ref", dest, WeakReferenceType.From(nbt));
			}
			else
			{
				ctx.Call("amethyst:core/ref/set", dest, src);
			}
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
