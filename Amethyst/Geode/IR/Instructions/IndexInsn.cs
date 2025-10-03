using Amethyst.Errors;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	public class IndexInsn(ValueRef dest, ValueRef index) : Instruction([dest, index])
	{
		public override string Name => "index";
		public override NBTType?[] ArgTypes => [null, NBTType.Int];

		public readonly TypeSpecifier ActualReturnType = dest.Type.Subtypes.Any() ? dest.Type.Subtypes.First() : throw new InvalidTypeError(dest.Type.ToString(), "list");
		public override TypeSpecifier ReturnType => new WeakReferenceTypeSpecifier(ActualReturnType);

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();

			if (val.Type is not ReferenceTypeSpecifier && val is DataTargetValue nbt)
			{
				val = WeakReferenceTypeSpecifier.From(nbt);
			}

			ctx.Call("amethyst:core/ref/index", WeakReferenceTypeSpecifier.From(ReturnValue.Expect<DataTargetValue>()), val, Arg<ValueRef>(1));
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0);

			if (val.Type is not ReferenceTypeSpecifier && val.Value is DataTargetValue list && index.Value is LiteralValue i)
			{
				if (i.Value is not NBTInt ind)
				{
					throw new InvalidTypeError(index.Type.ToString(), "int");
				}

				Remove();
				return WeakReferenceTypeSpecifier.From(list.Index(ind.Value, ActualReturnType));
			}

			return null;
		}
	}
}
