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

			if (val.Type is EntityType)
			{
				ReturnValue.Expect<DynamicValue>()
					.Add("entity @e[scores={amethyst_id=")
					.Add(val)
					.Add("},limit=1] ")
					.Add(prop);

				return;
			}

			if (val.Type is not ReferenceType && val is DataTargetValue nbt)
			{
				if (val is MacroValue)
				{
					throw new MacroPropertyError();
				}

				val = WeakReferenceType.From(nbt);
			}

			ReturnValue.Expect<DynamicValue>()
				.Add(val)
				.Add(".")
				.Add(prop);
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0);
			var prop = Arg<ValueRef>(1);

			ReturnValue.AddDependency(val);
			ReturnValue.AddDependency(prop);

			// I don't want to deal with nested Macroizer stuff
			if (val.Type is ReferenceType r && r.Inner is EntityType e)
			{
				throw new InvalidTypeError(prop.Type.ToString(), e.ToString());
			}

			if (prop.Value is LiteralValue l && val.Type is not EntityType)
			{
				if (l.Value is not NBTString name)
				{
					throw new InvalidTypeError(prop.Type.ToString(), "string");
				}

				// TODO: Probably should clean this up at some point
				if (val.Type is not ReferenceType && val.Value is DataTargetValue nbt)
				{
					Remove();
					return WeakReferenceType.From(nbt.Property(name, ActualReturnType));
				}
				else if (val.Value is MacroValue && val.Type is not ReferenceType)
				{
					throw new MacroPropertyError();
				}
				else if (!(val.Type is ReferenceType && val.Value is IConstantValue))
                {
                    throw new ReferenceError(val.Type.ToString());
                }
			}

			return new DynamicValue(ReturnType);
		}
	}
}
