using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public abstract class ComparisonInsn(ValueRef left, ValueRef right) : Simple2IntInsn<NBTBool>(left, right)
	{
		public override TypeSpecifier ReturnType => PrimitiveType.Bool;
		public abstract Comparison Op { get; }
		public virtual bool Invert { get; } = false;

		public override void Render(RenderContext ctx)
		{
			var left = Arg<ValueRef>(0).Expect().AsScore(ctx);
			var right = Arg<ValueRef>(1).Expect().AsScore(ctx);

			ReturnValue.Expect<LValue>().Store(new LiteralValue(false), ctx);

			var cmd = new Execute();
			(Invert ? cmd.Unless : cmd.If).Score(left.Target, left.Score, Op, right.Target, right.Score).Run(ctx.WithFaux(ctx => ReturnValue.Expect<LValue>().Store(new LiteralValue(true), ctx)).Single());
			ctx.Add(cmd);
		}

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap)
        {
            markOverlap(ReturnValue, Arg<ValueRef>(0));
			markOverlap(ReturnValue, Arg<ValueRef>(1));
		}
	}

	public class EqInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
	{
		public override string Name => "eq";
		public override Comparison Op => Comparison.Equal;
		public override NBTBool Compute(NBTInt left, NBTInt right) => left == right;
	}

	public class NeqInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
	{
		public override string Name => "neq";
		public override Comparison Op => Comparison.Equal;
		public override bool Invert => true;
		public override NBTBool Compute(NBTInt left, NBTInt right) => left != right;
	}

	public class LtInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
	{
		public override string Name => "lt";
		public override Comparison Op => Comparison.LessThan;
		public override NBTBool Compute(NBTInt left, NBTInt right) => left < right;
	}

	public class LteInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
	{
		public override string Name => "lte";
		public override Comparison Op => Comparison.LessThanOrEqual;
		public override NBTBool Compute(NBTInt left, NBTInt right) => left <= right;
	}

	public class GtInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
	{
		public override string Name => "gt";
		public override Comparison Op => Comparison.GreaterThan;
		public override NBTBool Compute(NBTInt left, NBTInt right) => left > right;
	}

	public class GteInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
	{
		public override string Name => "gte";
		public override Comparison Op => Comparison.GreaterThanOrEqual;
		public override NBTBool Compute(NBTInt left, NBTInt right) => left >= right;
	}
}
