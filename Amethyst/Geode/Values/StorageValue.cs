using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public class StorageValue(Storage storage, string path, TypeSpecifier type) : LValue
    {
        public readonly Storage Storage = storage;
        public readonly string Path = path;
        public override TypeSpecifier Type => type;

        public StorageValue Property(string member, TypeSpecifier type) => new(Storage, $"{Path}.{member}", type);
        public StorageValue Index(int index, TypeSpecifier type) => new(Storage, $"{Path}[{index}]", type);

        public override ScoreValue AsScore(RenderContext ctx)
        {
            throw new InvalidOperationException("Cannot implicitly convert a storage value to a score");
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

        public override FormattedText Render(FormattedText text) => text.Storage(Storage, Path);

        public override bool Equals(object? obj) => obj is StorageValue s && s.Storage == Storage && s.Path == Path;
        public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Execute().Store(Storage, Path, Type.EffectiveNumberType ?? NBTNumberType.Int, 1).Run(new Scoreboard.Players.Get(score.Target, score.Score)));
        public override void Store(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Set().Value(literal.ToString()));
        public override void Store(StorageValue score, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Set().From(score.Storage, score.Path));
        public override string ToString() => $"{Storage}.{Path}";
        public override int GetHashCode() => Storage.GetHashCode() * Path.GetHashCode() * Type.GetHashCode();

        public override void ListAdd(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Append().Value(literal.ToString()));
        public override void ListAdd(StorageValue storage, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Append().From(storage.Storage, storage.Path));
    }
}
