using Amethyst.Errors;
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
			var prop = Arg<ValueRef>(1).Expect();

			if (val.Type is not ReferenceType && val is DataTargetValue nbt)
			{
				if (val is MacroValue)
				{
					throw new MacroPropertyError();
				}

				val = WeakReferenceType.From(nbt);
			}

			ctx.Macroize([val, prop], (args, ctx) =>
			{
				ReturnValue.Expect<LValue>().Store(new LiteralValue($"{args[0]}.{args[1]}"), ctx);
			});
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0);
			var prop = Arg<ValueRef>(1);

			if (prop.Value is LiteralValue l && val.Type is not EntityType)
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
				else if (val.Value is MacroValue)
				{
					throw new MacroPropertyError();
				}
				else
                {
                    throw new ReferenceError(val.Type.ToString());
                }
			}

			return null;
		}
	}
}
