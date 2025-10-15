using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class IndexInsn(ValueRef dest, ValueRef index) : Instruction([dest, index])
	{
		public override string Name => "index";
		public override NBTType?[] ArgTypes => [null, NBTType.Int];

		public TypeSpecifier ActualReturnType
		{
			get
			{
				var type = Arg<ValueRef>(0).Type;

				if (type is ReferenceType r && r.Inner is ListType rl)
				{
					return rl.Inner;
				}
				else if (type is ListType l)
				{
					return l.Inner;
				}

				throw new InvalidTypeError(dest.Type.ToString(), "list");
			}
		}
		public override TypeSpecifier ReturnType => new WeakReferenceType(ActualReturnType);

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();

			if (val.Type is not ReferenceType && val is DataTargetValue nbt)
			{
				val = WeakReferenceType.From(nbt);
			}

			ctx.Call("amethyst:core/ref/index", WeakReferenceType.From(ReturnValue.Expect<DataTargetValue>()), val, Arg<ValueRef>(1));
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0);

			if (val.Type is not ReferenceType && val.Value is DataTargetValue list && index.Value is LiteralValue i)
			{
				if (i.Value is not NBTInt ind)
				{
					throw new InvalidTypeError(index.Type.ToString(), "int");
				}

				Remove();
				return WeakReferenceType.From(list.Index(ind.Value, ActualReturnType));
			}

			return null;
		}
	}
}
