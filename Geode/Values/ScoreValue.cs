using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Errors;
using Geode.Types;

namespace Geode.Values
{
	public class ScoreValue(IEntityTarget target, Score score, TypeSpecifier? type = null) : DataLValue
	{
		public readonly IEntityTarget Target = target;
		public readonly Score Score = score;
		public override string ToString() => $"@{Target.Get()}.{Score}";

		public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Scoreboard.Players.Operation(Target, Score, ScoreOperation.Assign, score.Target, score.Score));
		public override void Store(LiteralValue literal, RenderContext ctx)
		{
			var val = literal.Value.ToString();

			if (val is "true")
			{
				val = "1";
			}
			else if (val is "false")
			{
				val = "0";
			}

			ctx.Add(new Scoreboard.Players.Set(Target, Score, val));
		}
		public override void Store(DataTargetValue nbt, RenderContext ctx) => ctx.Add(new Execute().Store(Target, Score).Run(new DataCommand.Get(nbt.Target)));

		public override ScoreValue AsScore(RenderContext ctx) => this;

		public override void If(Action<Execute> apply, RenderContext ctx, int tmp = 0)
		{
			var cmd = new Execute().Unless.Score(Target, Score, 0);
			apply(cmd);
			ctx.Add(cmd);
		}

		public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Score(Target, Score);
		public override bool Equals(object? obj) => obj is ScoreValue s && s.Score == Score && s.Target.Get() == Target.Get();

		public override TypeSpecifier Type => type ?? PrimitiveType.Int;

		public override int GetHashCode() => HashCode.Combine(Target, Score);

		public override void ListAdd(LiteralValue literal, RenderContext ctx) => throw new InvalidTypeError("int", "list");
		public override void ListAdd(DataTargetValue nbt, RenderContext ctx) => throw new InvalidTypeError("int", "list");
	}
}
