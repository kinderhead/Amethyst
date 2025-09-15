using Amethyst.Errors;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
	public class IndexInsn(ValueRef dest, ValueRef index) : Instruction([dest, index])
	{
		public override string Name => "index";
		public override NBTType?[] ArgTypes => [null, NBTType.Int];
		public override TypeSpecifier ReturnType => dest.Type.Subtypes.Any() ? dest.Type.Subtypes.First() : throw new InvalidTypeError(dest.Type.ToString(), "list");

		public override void Render(RenderContext ctx)
		{
			throw new NotImplementedException();
		}

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			if (index.Value is LiteralValue i)
			{
				if (i.Value is not NBTInt val) throw new InvalidTypeError(index.Type.ToString(), "int");
				Remove();
				return Arg<ValueRef>(0).Expect<DataTargetValue>().Index(val.Value, ReturnType);
			}

			return null;
		}
	}
}
