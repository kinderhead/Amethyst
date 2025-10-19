using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode
{
	public interface IValueLike
	{
		IValue Expect();
	}

	public interface IValue : IValueLike
	{
		TypeSpecifier Type { get; }
		bool IsLiteral => this is IConstantValue;

		ScoreValue AsScore(RenderContext ctx);
		void If(Action<Execute> apply, RenderContext ctx, int tmp = 0);
		FormattedText Render(FormattedText text, RenderContext ctx);
	}

	public abstract class Value : IValue
	{
		public abstract TypeSpecifier Type { get; }

		public abstract ScoreValue AsScore(RenderContext ctx);
		public IValue Expect() => this;
		public abstract void If(Action<Execute> apply, RenderContext ctx, int tmp = 0);
		public abstract FormattedText Render(FormattedText text, RenderContext ctx);
	}

	public abstract class LValue : Value
	{
		public abstract void Store(IValue val, RenderContext ctx);
		public abstract void ListAdd(IValue val, RenderContext ctx);
	}

	public abstract class DataLValue : LValue
	{
		public override void Store(IValue val, RenderContext ctx)
		{
			if (val is LiteralValue literal)
			{
				Store(literal, ctx);
			}
			else if (val is ScoreValue score)
			{
				Store(score, ctx);
			}
			else if (val is DataTargetValue storage)
			{
				Store(storage, ctx);
			}
			else if (val is ConditionalValue cond)
			{
				Store(cond, ctx);
			}
			else if (val is MacroValue macro)
			{
				Store(macro, ctx);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public override void ListAdd(IValue val, RenderContext ctx)
		{
			if (val is LiteralValue literal)
			{
				ListAdd(literal, ctx);
			}
			else if (val is ScoreValue score)
			{
				ListAdd(score, ctx);
			}
			else if (val is DataTargetValue storage)
			{
				ListAdd(storage, ctx);
			}
			else if (val is ConditionalValue cond)
			{
				ListAdd(cond, ctx);
			}
			else if (val is MacroValue macro)
			{
				ListAdd(macro, ctx);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public abstract void Store(LiteralValue literal, RenderContext ctx);
		public abstract void Store(ScoreValue score, RenderContext ctx);
		public abstract void Store(DataTargetValue nbt, RenderContext ctx);
		public virtual void Store(MacroValue macro, RenderContext ctx) => Store(new LiteralValue(new NBTRawString(macro.GetMacro())), ctx);

		public virtual void Store(ConditionalValue cond, RenderContext ctx)
		{
			Store(new LiteralValue(false), ctx);
			cond.If(cmd => cmd.Run(ctx.WithFaux(i => Store(new LiteralValue(true), i)).Single()), ctx);
		}

		public abstract void ListAdd(LiteralValue literal, RenderContext ctx);

		public virtual void ListAdd(ScoreValue score, RenderContext ctx)
		{
			var tmp = ctx.Builder.TempStorage(PrimitiveType.Compound);
			tmp.Store(score, ctx);
			ListAdd(tmp, ctx);
		}

		public abstract void ListAdd(DataTargetValue nbt, RenderContext ctx);

		public virtual void ListAdd(ConditionalValue cond, RenderContext ctx)
		{
			var tmp = ctx.Builder.TempStorage(PrimitiveType.Compound);
			tmp.Store(cond, ctx);
			ListAdd(tmp, ctx);
		}

		public virtual void ListAdd(MacroValue macro, RenderContext ctx) => ListAdd(new LiteralValue(new NBTRawString(macro.GetMacro())), ctx);
	}
}
