using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

namespace Amethyst.Geode
{
    public abstract class Value
    {
        public abstract TypeSpecifier Type { get; }
        public bool IsLiteral => this is LiteralValue;

        public abstract ScoreValue AsScore(RenderContext ctx);
        public abstract Execute If(Execute cmd);
        public abstract FormattedText Render(FormattedText text);

        public override string ToString() => "";
        public abstract override bool Equals(object? obj);
        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Value left, Value right) => left.Equals(right);
        public static bool operator !=(Value left, Value right) => !left.Equals(right);
    }

    public class LiteralValue(NBTValue val) : Value
    {
        public readonly NBTValue Value = val;
        public override TypeSpecifier Type => new PrimitiveTypeSpecifier(Value.Type);

        public override ScoreValue AsScore(RenderContext ctx) => Value is NBTInt n ? ctx.Builder.Constant(n) : throw new InvalidOperationException($"\"{Value}\" is not an integer");
        public override Execute If(Execute cmd) => throw new NotImplementedException(); // Ideally this shouldn't happen
        public override bool Equals(object? obj) => obj is LiteralValue l && l.Value == Value;
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public override FormattedText Render(FormattedText text) => Value is NBTString str ? text.Text(str.Value) : text.Text(Value.ToString());
    }

    public class VoidValue : Value
    {
        public override TypeSpecifier Type => new VoidTypeSpecifier();
        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
        public override Execute If(Execute cmd) => throw new NotImplementedException(); // Ideally this shouldn't happen, also idk how to make the execute always fail consistently
        public override bool Equals(object? obj) => obj is VoidValue;
        public override int GetHashCode() => 0; // hmm
        public override FormattedText Render(FormattedText text) => text.Text("void");
    }

    public class StaticFunctionValue(NamespacedID id, FunctionTypeSpecifier type) : LiteralValue(new NBTString(id.ToString()))
    {
        public readonly NamespacedID ID = id;
        public override TypeSpecifier Type => type;
        public override string ToString() => ID.ToString();
        public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Type;
    }

    public class ConditionalValue(Func<Execute, Execute> apply) : Value
    {
        public readonly Func<Execute, Execute> Apply = apply;
        public override TypeSpecifier Type => PrimitiveTypeSpecifier.Bool;

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException("Cannot implicitly convert a conditional to a score");
        public override bool Equals(object? obj) => obj is ConditionalValue c && c.Apply == Apply; // I don't like this but oh well
        public override Execute If(Execute cmd) => Apply(cmd);
        public override int GetHashCode() => Apply.GetHashCode();

        public override FormattedText Render(FormattedText text)
        {
            throw new NotImplementedException("Cannot print conditionals at the moment. Assign to a variable for now");
        }
    }

    public abstract class LValue : Value
    {
        public void Store(Value val, RenderContext ctx)
        {
            if (val is LiteralValue literal) Store(literal, ctx);
            else if (val is ScoreValue score) Store(score, ctx);
            else if (val is StorageValue storage) Store(storage, ctx);
            else if (val is ConditionalValue cond) Store(cond, ctx);
            else throw new NotImplementedException();
        }

        public abstract void Store(LiteralValue literal, RenderContext ctx);
        public abstract void Store(ScoreValue score, RenderContext ctx);
        public abstract void Store(StorageValue score, RenderContext ctx);
        public virtual void Store(ConditionalValue cond, RenderContext ctx)
        {
            Store(new LiteralValue(false), ctx);
            ctx.Add(cond.If(new()).Run(ctx.WithFaux(i => Store(new LiteralValue(true), i)).Single()));
        }
    }

    public class ScoreValue(IEntityTarget target, Score score) : LValue
    {
        public readonly IEntityTarget Target = target;
        public readonly Score Score = score;
        public override string ToString() => $"@{Target.Get()}.{Score}";

        public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Scoreboard.Players.Operation(Target, Score, ScoreOperation.Assign, score.Target, score.Score));
        public override void Store(LiteralValue literal, RenderContext ctx) => ctx.Add(new Scoreboard.Players.Set(Target, Score, literal.ToString()));
        public override void Store(StorageValue score, RenderContext ctx) => ctx.Add(new Execute().Store(Target, Score).Run(new DataCommand.Get(score.Storage, score.Path)));

        public override ScoreValue AsScore(RenderContext ctx) => this;
        public override Execute If(Execute cmd) => cmd.Unless.Score(Target, Score, 0);
        public override FormattedText Render(FormattedText text) => text.Score(Target, Score);
        public override bool Equals(object? obj) => obj is ScoreValue s && s.Score == Score && s.Target.Get() == Target.Get();

        public override TypeSpecifier Type => PrimitiveTypeSpecifier.Int;

        public override int GetHashCode() => HashCode.Combine(Target, Score);
    }

    public class StorageValue(Storage storage, string path, TypeSpecifier type) : LValue
    {
        public readonly Storage Storage = storage;
        public readonly string Path = path;
        public override TypeSpecifier Type => type;

        public StorageValue Property(string member, TypeSpecifier type) => new(Storage, $"{Path}.{member}", type);

        public override ScoreValue AsScore(RenderContext ctx)
        {
            throw new InvalidOperationException("Cannot implicitly convert a storage value to a score");
            //// No type checking because this acts like a cast to int
            //var val = ctx.Builder.Temp(tmp);
            //val.Store(this, ctx);
            //return val;
        }

        public override Execute If(Execute cmd) => throw new InvalidOperationException("Cannot use storage for conditions");
        public override FormattedText Render(FormattedText text) => text.Storage(Storage, Path);

        public override bool Equals(object? obj) => obj is StorageValue s && s.Storage == Storage && s.Path == Path;
        public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Execute().Store(Storage, Path, Type.EffectiveNumberType, 1).Run(new Scoreboard.Players.Get(score.Target, score.Score)));
        public override void Store(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Set().Value(literal.ToString()));
        public override void Store(StorageValue score, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Set().From(score.Storage, score.Path));
        public override string ToString() => $"{Storage}.{Path}";
        public override int GetHashCode() => Storage.GetHashCode() * Path.GetHashCode() * Type.GetHashCode();
    }
}
