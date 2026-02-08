using Amethyst.Errors;
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
			var index = Arg<ValueRef>(1).Expect();

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
				.Add("[")
				.Add(index)
				.Add("]");
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0);
			var index = Arg<ValueRef>(1);

			if (val.Value is MacroValue && val.Type is not ReferenceType)
			{
				throw new MacroPropertyError();
			}

			if (val.Type is not ReferenceType && val.Value is DataTargetValue list && index.Value is LiteralValue i)
			{
				if (i.Value is not NBTInt ind)
				{
					throw new InvalidTypeError(index.Type.ToString(), "int");
				}

				Remove();
				return WeakReferenceType.From(list.Index(ind.Value, ActualReturnType));
			}

			ReturnValue.AddDependency(val);
			ReturnValue.AddDependency(index);

			return new DynamicValue(ReturnType);
		}
	}
}
