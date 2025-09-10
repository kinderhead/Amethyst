using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode
{
    public abstract class Value
    {
        public abstract TypeSpecifier Type { get; }
        public bool IsLiteral => this is LiteralValue;

        public abstract ScoreValue AsScore(RenderContext ctx);
        public abstract Execute If(Execute cmd, RenderContext ctx, int tmp = 0);
        public abstract FormattedText Render(FormattedText text);

        public override string ToString() => "";
        public abstract override bool Equals(object? obj);
        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Value left, Value right) => left.Equals(right);
        public static bool operator !=(Value left, Value right) => !left.Equals(right);
    }

    public abstract class LValue : Value
    {
        public void Store(Value val, RenderContext ctx)
        {
            if (val is LiteralValue literal) Store(literal, ctx);
            else if (val is ScoreValue score) Store(score, ctx);
            else if (val is StorageValue storage) Store(storage, ctx);
            else if (val is ConditionalValue cond) Store(cond, ctx);
            else if (val is MacroValue macro) Store(macro, ctx);
            else throw new NotImplementedException();
        }

        public abstract void Store(LiteralValue literal, RenderContext ctx);
        public abstract void Store(ScoreValue score, RenderContext ctx);
        public abstract void Store(StorageValue score, RenderContext ctx);
        public virtual void Store(MacroValue macro, RenderContext ctx) => Store(new LiteralValue(new NBTRawString($"$({macro.Name})")), ctx);
        public virtual void Store(ConditionalValue cond, RenderContext ctx)
        {
            Store(new LiteralValue(false), ctx);
            ctx.Add(cond.If(new(), ctx).Run(ctx.WithFaux(i => Store(new LiteralValue(true), i)).Single()));
        }

        public void ListAdd(Value val, RenderContext ctx)
        {
            if (val is LiteralValue literal) ListAdd(literal, ctx);
            else if (val is ScoreValue score) ListAdd(score, ctx);
            else if (val is StorageValue storage) ListAdd(storage, ctx);
            else if (val is ConditionalValue cond) ListAdd(cond, ctx);
            else if (val is MacroValue macro) ListAdd(macro, ctx);
            else throw new NotImplementedException();
        }

        public abstract void ListAdd(LiteralValue literal, RenderContext ctx);

        public virtual void ListAdd(ScoreValue score, RenderContext ctx)
        {
            var tmp = ctx.Builder.TempStorage(0, PrimitiveTypeSpecifier.Compound);
            tmp.Store(score, ctx);
            ListAdd(tmp, ctx);
        }

        public abstract void ListAdd(StorageValue storage, RenderContext ctx);

        public virtual void ListAdd(ConditionalValue cond, RenderContext ctx)
        {
            var tmp = ctx.Builder.TempStorage(0, PrimitiveTypeSpecifier.Compound);
            tmp.Store(cond, ctx);
            ListAdd(tmp, ctx);
        }

        public virtual void ListAdd(MacroValue macro, RenderContext ctx) => ListAdd(new LiteralValue(new NBTRawString($"$({macro.Name})")), ctx);
    }
}
