using Amethyst.AST;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen
{
	public abstract class Value
	{
		public abstract TypeSpecifier Type { get; }

		public abstract Command ReadTo(Storage storage, string path);
		public abstract Command ReadTo(IEntityTarget target, Score score);
		public abstract Command Get();
		public abstract FormattedText Format(FormattedText text);

		public virtual ScoreValue AsScore(FunctionContext ctx)
		{
			//if (!NBTValue.IsOperableType(Type.Type)) throw new InvalidOperationException();
			var tmp = ctx.AllocTempScore();
			tmp.Store(ctx, this);
			return tmp;
		}
	}

	public class LiteralValue(NBTValue val) : Value
	{
		public readonly NBTValue Value = val;
		public override TypeSpecifier Type => new PrimitiveTypeSpecifier(Value.Type);

		public int ToScoreInt()
		{
			if (NBTValue.IsOperableType(Value.Type) && Value is INBTNumber num) return Convert.ToInt32(num.RawValue);
			else if (Value is ICollection l) return l.Count;
			else if (Value is NBTString s) return s.Value.Length;
			throw new NotImplementedException();
		}

		public override FormattedText Format(FormattedText text) => Value is NBTString str ? text.Text(str.Value) : text.Text(Value.ToString());
		public override Command Get() => throw new InvalidOperationException(); // Maybe implement this
		public override Command ReadTo(Storage storage, string path) => new DataCommand.Modify(storage, path).Set().Value(Value.ToString());

		public override Command ReadTo(IEntityTarget target, Score score) => new Scoreboard.Players.Set(target, score, ToScoreInt());
	}

	public class VoidValue : Value
	{
		public override TypeSpecifier Type => new VoidTypeSpecifier();

		public override FormattedText Format(FormattedText text) => text.Text("void");
		public override Command Get() => throw new InvalidOperationException();
		public override Command ReadTo(Storage storage, string path) => throw new InvalidOperationException();
		public override Command ReadTo(IEntityTarget target, Score score) => throw new InvalidOperationException();
	}

	public class StaticFunctionValue(NamespacedID id, FunctionTypeSpecifier type) : LiteralValue(new NBTString(id.ToString()))
	{
		public readonly NamespacedID ID = id;
		public override TypeSpecifier Type => type;
		public FunctionTypeSpecifier FuncType => (FunctionTypeSpecifier)Type;
	}

	public abstract class MutableValue : Value
	{
		public abstract void Store(FunctionContext ctx, Value val);
		public abstract void Store(FunctionContext ctx, Value val, NBTNumberType type);

		public abstract Execute WriteFrom(Execute cmd, bool result = true);
	}

	public class StorageValue(Storage storage, string path, TypeSpecifier type) : MutableValue
	{
		public readonly Storage Storage = storage;
		public readonly string Path = path;
		public override TypeSpecifier Type => type;

		public override FormattedText Format(FormattedText text) => text.Storage(Storage, Path);
		public override Command Get() => new DataCommand.Get(Storage, Path);

		public override void Store(FunctionContext ctx, Value val)
		{
			ctx.Add(new StoreInstruction(ctx.CurrentLocator.Location, this, val));
		}

		public override void Store(FunctionContext ctx, Value val, NBTNumberType type)
		{
			ctx.Add(new StoreAsTypeInstruction(ctx.CurrentLocator.Location, this, val, type));
		}

		public override Command ReadTo(Storage storage, string path) => new DataCommand.Modify(storage, path).Set().From(Storage, Path);
		public override Command ReadTo(IEntityTarget target, Score score)
		{
			//if (!NBTValue.IsOperableType(Type.Type)) throw new InvalidOperationException();

			return new Execute().Store(target, score).Run(Get());
		}

		public override Execute WriteFrom(Execute cmd, bool result = true) => cmd.Store(Storage, Path, (NBTNumberType)Type.Type, 1, result);
	}

	public class ScoreValue(IEntityTarget target, Score score) : MutableValue
	{
		public readonly IEntityTarget Target = target;
		public readonly Score Score = score;

		public override TypeSpecifier Type => new PrimitiveTypeSpecifier(NBTType.Int);
		public override Command Get() => new Scoreboard.Players.Get(Target, Score);
		public override void Store(FunctionContext ctx, Value val) => ctx.Add(new StoreScoreInstruction(ctx.CurrentLocator.Location, this, val));
		public override void Store(FunctionContext ctx, Value val, NBTNumberType type) => Store(ctx, val);
		public override Command ReadTo(Storage storage, string path) => new Execute().Store(storage, path, NBTNumberType.Int, 1).Run(Get());
		public override Command ReadTo(IEntityTarget target, Score score) => new Scoreboard.Players.Operation(target, score, ScoreOperation.Assign, Target, Score);
		public override ScoreValue AsScore(FunctionContext ctx) => this;
		public override FormattedText Format(FormattedText text) => text.Score(Target, Score);
		public override Execute WriteFrom(Execute cmd, bool result = true) => cmd.Store(Target, Score, result);
	}
}
