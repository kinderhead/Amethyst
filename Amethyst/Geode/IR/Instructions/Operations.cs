using Amethyst.Geode.Types;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.IR.Instructions
{
	public abstract class OpInsn(ValueRef left, ValueRef right) : Simple2IntInsn<NBTInt>(left, right)
	{
		public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Int;
		public abstract bool IsCommunitive { get; }
		public abstract ScoreOperation Op { get; }

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap)
		{
			if (!(Arguments[0] is ValueRef v0 && tryLink(ReturnValue, v0)))
			{
				if (IsCommunitive && Arguments[1] is ValueRef v1)
				{
					tryLink(ReturnValue, v1);
					Arguments[1] = Arguments[0];
					Arguments[0] = v1;
				}
			}

			if (Arguments[1] is ValueRef v)
			{
				markOverlap(ReturnValue, v);
			}
		}

		public override void Render(RenderContext ctx)
		{
			var left = Arg<ValueRef>(0).Expect(NBTType.Int);
			var right = Arg<ValueRef>(1).Expect(NBTType.Int).AsScore(ctx);
			var ret = ReturnValue.Expect(NBTType.Int).AsScore(ctx);

			if (left != ret)
			{
				ret.Store(left, ctx);
			}

			ctx.Add(new Scoreboard.Players.Operation(ret.Target, ret.Score, Op, right.Target, right.Score));
		}
	}

	public class AddInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
	{
		public override string Name => "add";
		public override bool IsCommunitive => true;
		public override ScoreOperation Op => ScoreOperation.Add;
		public override NBTInt Compute(NBTInt left, NBTInt right) => left + right;
	}

	public class SubInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
	{
		public override string Name => "sub";
		public override bool IsCommunitive => false;
		public override ScoreOperation Op => ScoreOperation.Sub;
		public override NBTInt Compute(NBTInt left, NBTInt right) => left - right;
	}

	public class MulInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
	{
		public override string Name => "mul";
		public override bool IsCommunitive => true;
		public override ScoreOperation Op => ScoreOperation.Mul;
		public override NBTInt Compute(NBTInt left, NBTInt right) => left * right;
	}

	public class DivInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
	{
		public override string Name => "div";
		public override bool IsCommunitive => false;
		public override ScoreOperation Op => ScoreOperation.Div;
		public override NBTInt Compute(NBTInt left, NBTInt right) => left / right;
	}

	public class ModInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
	{
		public override string Name => "mod";
		public override bool IsCommunitive => false;
		public override ScoreOperation Op => ScoreOperation.Mod;
		public override NBTInt Compute(NBTInt left, NBTInt right) => left % right;
	}
}
