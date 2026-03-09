using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.Types;
using Geode.Values;

namespace Geode
{
	public interface IValueLike
	{
		TypeSpecifier Type { get; }
		IValue? Value { get; }

		ValueRef ToValueRef();
	}

	public static class ValueExtensions
	{
		extension(IValueLike self)
		{
			public IValue Expect(NBTType type)
			{
				var val = self.Value;

				if (val is null || val.Type.EffectiveType != type)
				{
					throw new InvalidTypeError(val is null ? "<error>" : (Enum.GetName(val.Type.EffectiveType)?.ToLower() ?? "<error>"), Enum.GetName(type)?.ToLower() ?? "<error>");
				}

				return val;
			}

			public T Expect<T>() where T : class, IValue
			{
				if (self.Value is T val)
				{
					return val;
				}

#if DEBUG
				System.Diagnostics.Debugger.Break();
#endif

				throw new InvalidTypeError(self.Value?.GetType().Name.ToLower() ?? "<error>", typeof(T).Name.ToLower());
			}

			public IValue Expect() => self.Expect<IValue>();
		}
	}

	public interface IValue : IValueLike
	{
		bool IsLiteral => this is IConstantValue;

		ScoreValue AsScore(RenderContext ctx);
		FormattedText Render(FormattedText text, RenderContext ctx);
	}

	public abstract class Value : IValue
	{
		public abstract TypeSpecifier Type { get; }
		IValue? IValueLike.Value => this;

		public abstract ScoreValue AsScore(RenderContext ctx);
		public abstract FormattedText Render(FormattedText text, RenderContext ctx);
		public ValueRef ToValueRef() => new(this);
	}

	public abstract class LValue : Value
	{
		public abstract void Store(IValue val, RenderContext ctx);
		public abstract void ListAdd(IValue val, RenderContext ctx);
		public abstract Execute StoreExecute(bool result = true);
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
			else if (val is MacroValue macro)
			{
				Store(macro, ctx);
			}
			else if (val is StoreableValue s)
			{
				Store(s.AsStoreable(), ctx);
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
		public virtual void Store(MacroValue macro, RenderContext ctx) => Store(LiteralValue.Raw(macro.GetMacro()), ctx);

		public abstract void ListAdd(LiteralValue literal, RenderContext ctx);

		public virtual void ListAdd(ScoreValue score, RenderContext ctx)
		{
			var tmp = ctx.Builder.TempStorage(PrimitiveType.Compound);
			tmp.Store(score, ctx);
			ListAdd(tmp, ctx);
		}

		public abstract void ListAdd(DataTargetValue nbt, RenderContext ctx);

		public virtual void ListAdd(MacroValue macro, RenderContext ctx) => ListAdd(LiteralValue.Raw(macro.GetMacro()), ctx);
	}
}
