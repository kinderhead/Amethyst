using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using System;

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

    public class LiteralValue(NBTValue val, TypeSpecifier? type = null) : Value
    {
        public readonly NBTValue Value = val;
        public override TypeSpecifier Type => type ?? new PrimitiveTypeSpecifier(Value.Type);

        public override ScoreValue AsScore(RenderContext ctx) => Value is NBTInt n ? ctx.Builder.Constant(n) : throw new InvalidOperationException($"\"{Value}\" is not an integer");
        public override Execute If(Execute cmd) => throw new NotImplementedException(); // Ideally this shouldn't happen
        public override bool Equals(object? obj) => obj is LiteralValue l && l.Value == Value;
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public override FormattedText Render(FormattedText text) => Value is NBTString str ? text.Text(str.Value) : text.Text(Value.ToString());
    }

    public class MacroValue(string name, TypeSpecifier type) : Value
    {
        public readonly string Name = name;
        public override TypeSpecifier Type => type;

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
        public override bool Equals(object? obj) => this is MacroValue m && m.Name == Name;
        public override Execute If(Execute cmd) => throw new InvalidOperationException();
        public override FormattedText Render(FormattedText text) => text.Text($"$({Name})");
        public override int GetHashCode() => HashCode.Combine(Name, Type);
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

    public class FunctionValue(NamespacedID id, FunctionTypeSpecifier type) : LiteralValue(new NBTString(id.ToString()))
    {
        public readonly NamespacedID ID = id;
        public override TypeSpecifier Type => type;
        public override string ToString() => ID.ToString();
        public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Type;

        public virtual void Call(RenderContext ctx, IEnumerable<ValueRef> args)
        {
			Value? processedMacros = null;

            if (args.Count() != FuncType.Parameters.Length) throw new MismatchedArgumentCountError(FuncType.Parameters.Length, args.Count());

			if (args.Any())
			{
				var processedArgs = new Dictionary<string, ValueRef>();
				var macros = new Dictionary<string, ValueRef>();

				foreach (var (param, val) in FuncType.Parameters.Zip(args))
				{
					if (param.Modifiers.HasFlag(AST.ParameterModifiers.Macro)) macros.Add(param.Name, val);
					else processedArgs.Add(param.Name, val);
				}

				if (processedArgs.Count != 0) ctx.StoreCompound(new(GeodeBuilder.RuntimeID, "stack[-1].args", PrimitiveTypeSpecifier.Compound), processedArgs, setEmpty: false);
				if (macros.Count != 0) processedMacros = ctx.StoreCompoundOrReturnConstant(new(GeodeBuilder.RuntimeID, "stack[-1].macros", PrimitiveTypeSpecifier.Compound), macros); // Minecraft throws an error if there are mismatched macro args, so we always reset it
			}

			if (processedMacros is StorageValue s) ctx.Add(new FunctionCommand(ID, s.Storage, s.Path));
			else if (processedMacros is LiteralValue l) ctx.Add(new FunctionCommand(ID, (NBTCompound)l.Value));
			else ctx.Add(new FunctionCommand(ID));
		}
    }

    public class MethodValue(FunctionValue val, ValueRef self) : FunctionValue(val.ID, new(val.FuncType.Modifiers, val.FuncType.ReturnType, val.FuncType.Parameters[1..]))
    {
        public readonly FunctionValue BaseFunction = val;
        public readonly ValueRef Self = self;

		public override void Call(RenderContext ctx, IEnumerable<ValueRef> args)
		{
			BaseFunction.Call(ctx, [Self, ..args]);
		}
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
            else if (val is MacroValue macro) Store(macro, ctx);
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

        public virtual void Store(MacroValue macro, RenderContext ctx) => Store(new LiteralValue(new NBTRawString($"$({macro.Name})")), ctx);
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
        public StorageValue Index(int index, TypeSpecifier type) => new(Storage, $"{Path}[{index}]", type);

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
        public override void Store(ScoreValue score, RenderContext ctx) => ctx.Add(new Execute().Store(Storage, Path, Type.EffectiveNumberType ?? NBTNumberType.Int, 1).Run(new Scoreboard.Players.Get(score.Target, score.Score)));
        public override void Store(LiteralValue literal, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Set().Value(literal.ToString()));
        public override void Store(StorageValue score, RenderContext ctx) => ctx.Add(new DataCommand.Modify(Storage, Path).Set().From(score.Storage, score.Path));
        public override string ToString() => $"{Storage}.{Path}";
        public override int GetHashCode() => Storage.GetHashCode() * Path.GetHashCode() * Type.GetHashCode();
    }
}
