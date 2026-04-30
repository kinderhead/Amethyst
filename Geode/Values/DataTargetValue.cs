using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;

namespace Geode.Values
{
	public abstract class DataTargetValue(TypeSpecifier type) : DataLValue(type), IValueWithProperties<DataTargetValue>
	{
		public abstract IDataTarget Target { get; }

		public abstract DataTargetValue Property(string member, TypeSpecifier type);
		public abstract DataTargetValue Index(int index, TypeSpecifier type);

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException("Cannot implicitly convert an NBT value to a score");//// No type checking because this acts like a cast to int//var val = ctx.Builder.Temp(tmp);//val.Store(this, ctx);//return val;

		public override void Store(IValue val, RenderContext ctx)
		{
			if (val is IDataWritable data)
			{
				data.StoreTo(this, ctx);
			}
			else
			{
				base.Store(val, ctx);
			}
		}

		public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Execute().Store(Target, Type.EffectiveNumberType ?? NBTNumberType.Int, 1).Run(new Scoreboard.Players.Get(score.Target, score.Score)));
		public override void Store(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Set().Value(literal.Value.ToString()));
		public override void Store(DataTargetValue nbt, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Set().From(nbt.Target));
		public override Execute StoreExecute(bool result = true) => new Execute().Store(Target, Type.EffectiveNumberType ?? NBTNumberType.Int, 1, result);

		public override void ListAdd(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Append().Value(literal.Value.ToString()));
		public override void ListAdd(DataTargetValue nbt, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Target).Append().From(nbt.Target));

		public override FormattedText Render(FormattedText text, RenderContext ctx) => text.NBT(Target, Type.EffectiveType == NBTType.String && ctx.Builder.Options.PackVersion >= new PackVersion(101, 0));
	}
}
