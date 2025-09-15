using Amethyst.Errors;
using Amethyst.Geode.Types;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public class ScoreValue(IEntityTarget target, Score score) : LValue
    {
        public readonly IEntityTarget Target = target;
        public readonly Score Score = score;
        public override string ToString() => $"@{Target.Get()}.{Score}";

        public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Scoreboard.Players.Operation(Target, Score, ScoreOperation.Assign, score.Target, score.Score));
        public override void Store(LiteralValue literal, RenderContext ctx) => ctx.Add(new Scoreboard.Players.Set(Target, Score, literal.ToString()));
        public override void Store(DataTargetValue nbt, RenderContext ctx) => ctx.Add(new Execute().Store(Target, Score).Run(new DataCommand.Get(nbt.Target)));

        public override ScoreValue AsScore(RenderContext ctx) => this;
        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0) => cmd.Unless.Score(Target, Score, 0);
        public override FormattedText Render(FormattedText text) => text.Score(Target, Score);
        public override bool Equals(object? obj) => obj is ScoreValue s && s.Score == Score && s.Target.Get() == Target.Get();

        public override TypeSpecifier Type => PrimitiveTypeSpecifier.Int;

        public override int GetHashCode() => HashCode.Combine(Target, Score);

        public override void ListAdd(LiteralValue literal, RenderContext ctx) => throw new InvalidTypeError("int", "list");
        public override void ListAdd(DataTargetValue nbt, RenderContext ctx) => throw new InvalidTypeError("int", "list");
    }
}
