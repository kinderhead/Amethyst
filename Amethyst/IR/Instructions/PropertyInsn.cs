using Amethyst.IR.Types;
using Datapack.Net.Data;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR.Instructions
{
	public class PropertyInsn(ValueRef val, ValueRef prop, TypeSpecifier destType) : Instruction([val, prop])
	{
		public override string Name => "prop";
		public override NBTType?[] ArgTypes => [null, NBTType.String];
		public TypeSpecifier ActualReturnType => destType;
		public override TypeSpecifier ReturnType => new WeakReferenceType(ActualReturnType);

		public override void Render(RenderContext ctx)
		{
			var val = Arg<ValueRef>(0).Expect();

			if (val.Type is not ReferenceType && val is DataTargetValue nbt)
			{
				val = WeakReferenceType.From(nbt);
			}

			ctx.Call("amethyst:core/ref/property", WeakReferenceType.From(ReturnValue.Expect<DataTargetValue>()), val, Arg<ValueRef>(1));
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0);
			var prop = Arg<ValueRef>(1);

			if (prop.Value is LiteralValue l)
			{
				if (l.Value is not NBTString name)
				{
					throw new InvalidTypeError(prop.Type.ToString(), "string");
				}

				Remove();

				if (val.Type is not ReferenceType && val.Value is DataTargetValue nbt)
				{
					return WeakReferenceType.From(nbt.Property(name, ActualReturnType));
				}
				else if (val.Type is ReferenceType && val.Value is IConstantValue v)
				{
					return new LiteralValue(new NBTString($"{v.Value}.{name.Value}"), new WeakReferenceType(ActualReturnType));
				}
			}

			return null;
		}
	}
}
