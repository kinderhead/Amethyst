using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
    public abstract class DataTargetValue : DataLValue, IValueWithProperties<DataTargetValue>
    {
        public abstract IDataTarget Target { get; }

        public abstract DataTargetValue Property(string member, TypeSpecifier type);
        public abstract DataTargetValue Index(int index, TypeSpecifier type);

        public override ScoreValue AsScore(RenderContext ctx)
        {
            throw new InvalidOperationException("Cannot implicitly convert an NBT value to a score");
            //// No type checking because this acts like a cast to int
            //var val = ctx.Builder.Temp(tmp);
            //val.Store(this, ctx);
            //return val;
        }

        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0)
        {
            var val = ctx.Builder.Temp(tmp); // Find some way to put it in a register nicely
            val.Store(this, ctx);
            return val.If(cmd, ctx, tmp + 1);
        }

        public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Execute().Store(Target, Type.EffectiveNumberType ?? NBTNumberType.Int, 1).Run(new Scoreboard.Players.Get(score.Target, score.Score)));
        public override void Store(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Set().Value(literal.ToString()));
        public override void Store(DataTargetValue nbt, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Set().From(nbt.Target));
        public override void Store(PropertyValue prop, RenderContext ctx) => prop.Get(this, ctx);

        public override void ListAdd(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Append().Value(literal.ToString()));
        public override void ListAdd(DataTargetValue nbt, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Append().From(nbt.Target));
    }

    public class RawDataTargetValue(string target, TypeSpecifier type) : DataTargetValue
    {
        public readonly string RawTarget = target;
        public override IDataTarget Target => new RawDataTarget(RawTarget);

        public override TypeSpecifier Type => type;

        public override bool Equals(object? obj) => obj is RawDataTargetValue r && r.RawTarget == RawTarget;
        public override int GetHashCode() => RawTarget.GetHashCode() * 17;
        public override DataTargetValue Index(int index, TypeSpecifier type) => new RawDataTargetValue($"{RawTarget}[{index}]", type);
        public override DataTargetValue Property(string member, TypeSpecifier type) => new RawDataTargetValue($"{RawTarget}.{member}", type);

        public override FormattedText Render(FormattedText text, RenderContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
