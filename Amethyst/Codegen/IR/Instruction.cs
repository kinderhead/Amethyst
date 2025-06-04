using Amethyst.AST;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen.IR
{
	public abstract class Instruction(LocationRange loc)
	{
		public readonly LocationRange Location = loc;
		public FunctionContext.Frame Ctx { get => ctx ?? throw new InvalidOperationException(); set => ctx = value; }
		public readonly List<Command> Commands = [];

		private FunctionContext.Frame? ctx = null;

		protected void Prepend(Command cmd) => Commands.Insert(0, cmd);
		protected void Add(Command cmd) => Commands.Add(cmd);
		protected void Add(IEnumerable<Command> cmd) => Commands.AddRange(cmd);

		public abstract void Build();
	}

	public class RawCommandInstruction(LocationRange loc, string cmd) : Instruction(loc)
	{
		public readonly string Command = cmd;

		public override void Build()
		{
			Add(new RawCommand(Command));
		}
	}

	public class StoreInstruction(LocationRange loc, StorageValue dest, Value val) : Instruction(loc)
	{
		public readonly StorageValue Dest = dest;
		public readonly Value Value = val;

		public override void Build()
		{
			Add(Value.ReadTo(Dest.Storage, Dest.Path));
		}
	}

	public class StoreAsTypeInstruction(LocationRange loc, StorageValue dest, Value val, NBTNumberType type) : Instruction(loc)
	{
		public readonly StorageValue Dest = dest;
		public readonly Value Value = val;
		public readonly NBTNumberType Type = type;

		public override void Build()
		{
			Add(new Execute().Store(Dest.Storage, Dest.Path, Type, 1).Run(Value.Get()));
		}
	}

	public class StoreIfSuccessInstruction(LocationRange loc, Execute exec, ScoreValue dest, Value val) : Instruction(loc)
	{
		public readonly Execute Command = exec;
		public readonly ScoreValue Dest = dest;
		public readonly Value Value = val;

		public override void Build()
		{
			Add(Command.Run(Value.ReadTo(Dest.Target, Dest.Score)));
		}
	}

	public class StoreScoreInstruction(LocationRange loc, ScoreValue dest, Value val) : Instruction(loc)
	{
		public readonly ScoreValue Dest = dest;
		public readonly Value Value = val;

		public override void Build()
		{
			Add(Value.ReadTo(Dest.Target, Dest.Score));
		}
	}

	public class ScoreOperationInstruction(LocationRange loc, ScoreValue dest, ScoreOperation op, ScoreValue val) : Instruction(loc)
	{
		public readonly ScoreValue Dest = dest;
		public readonly ScoreOperation Op = op;
		public readonly ScoreValue Value = val;

		public override void Build()
		{
			Add(new Scoreboard.Players.Operation(Dest.Target, Dest.Score, Op, Value.Target, Value.Score));
		}
	}

	public class CallInstruction(LocationRange loc, NamespacedID id) : Instruction(loc)
	{
		public readonly NamespacedID ID = id;

		public override void Build()
		{
			// Fix this later
			Add(new FunctionCommand(new(ID, true)));
		}
	}

	public class TellrawInstruction(LocationRange loc, FormattedText text) : Instruction(loc)
	{
		public readonly FormattedText Text = text;

		public override void Build()
		{
			Add(new TellrawCommand(new TargetSelector(TargetType.a), Text));
		}
	}

	public class CompareJumpInstruction(LocationRange loc, Execute cmd, SubFunction func) : Instruction(loc)
	{
		public readonly Execute Command = cmd;
		public readonly SubFunction Func = func;

		public override void Build()
		{
			Add(Func.Call(Command));
		}
	}

	public class StackPushInstruction(LocationRange loc, StorageValue val) : Instruction(loc)
	{
		public readonly StorageValue Value = val;

		public override void Build()
		{
			Add(new DataCommand.Modify(new Storage(Compiler.RuntimeID), "stack").Append().From(Value.Storage, Value.Path));
		}
	}
}
