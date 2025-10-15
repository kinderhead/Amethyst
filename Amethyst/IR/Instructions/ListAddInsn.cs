using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class ListAddInsn(ValueRef list, ValueRef val) : Instruction([list, val])
	{
		public override string Name => "add";
		public override NBTType?[] ArgTypes => [null, null];
		public override TypeSpecifier ReturnType => new VoidType();

		public override void Render(RenderContext ctx)
		{
			var list = Arg<ValueRef>(0).Expect();
			var val = Arg<ValueRef>(1).Expect();

			if (list.Type is ListType && list is LValue l)
			{
				l.ListAdd(val, ctx);
			}
			else if (list is IConstantValue c && c.Value is NBTString ptr)
			{
				new RawDataTargetValue(ptr.Value, PrimitiveType.List).ListAdd(val, ctx);
			}
			else if (val is DataTargetValue nbt)
			{
				ctx.Call("amethyst:core/ref/append-ref", list, WeakReferenceType.From(nbt));
			}
			else
			{
				ctx.Call("amethyst:core/ref/append", list, val);
			}
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx) => new VoidValue();
	}
}
