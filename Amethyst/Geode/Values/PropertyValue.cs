using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Geode.Values
{
	public abstract class PropertyValue(TypeSpecifier type) : LValue
	{
		public override TypeSpecifier Type => type;

		public override ScoreValue AsScore(RenderContext ctx)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object? obj) => ReferenceEquals(this, obj); // Not sure how to go about this
		public override int GetHashCode() => base.GetHashCode();

        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0)
		{
			throw new NotImplementedException();
		}

        public abstract void Get(DataTargetValue dest, RenderContext ctx);
        public abstract void Set(Value val, RenderContext ctx);
        public new abstract void ListAdd(Value val, RenderContext ctx);

        public override void ListAdd(LiteralValue literal, RenderContext ctx) => ListAdd(literal, ctx);
        public override void ListAdd(DataTargetValue nbt, RenderContext ctx) => ListAdd(nbt, ctx);
		public override void ListAdd(ConditionalValue cond, RenderContext ctx) => ListAdd(cond, ctx);
		public override void ListAdd(MacroValue macro, RenderContext ctx) => ListAdd(macro, ctx);
		public override void ListAdd(ScoreValue score, RenderContext ctx) => ListAdd(score, ctx);

        public override FormattedText Render(FormattedText text, RenderContext ctx)
		{
			var tmp = ctx.Builder.TempStorage(Type);
			Get(tmp, ctx);
			return tmp.Render(text, ctx);
        }

		public override void Store(LiteralValue literal, RenderContext ctx) => Set(literal, ctx);
		public override void Store(ScoreValue score, RenderContext ctx) => Set(score, ctx);
		public override void Store(DataTargetValue nbt, RenderContext ctx) => Set(nbt, ctx);
		public override void Store(ConditionalValue cond, RenderContext ctx) => Set(cond, ctx);
		public override void Store(MacroValue macro, RenderContext ctx) => Set(macro, ctx);
    }
}
