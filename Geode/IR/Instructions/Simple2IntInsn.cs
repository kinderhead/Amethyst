using Datapack.Net.Data;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public abstract class Simple2IntInsn<TOut>(ValueRef left, ValueRef right) : Instruction([left, right]) where TOut : NBTValue
	{
		public override NBTType?[] ArgTypes => [NBTType.Int, NBTType.Int];
		public abstract TOut Compute(NBTInt left, NBTInt right);

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			if (AreArgsLiteral(out NBTInt left, out NBTInt right))
			{
				return new LiteralValue(Compute(left, right));
			}

			return null;
		}
	}
}
