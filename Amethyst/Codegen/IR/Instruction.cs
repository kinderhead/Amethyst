using Amethyst.AST;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
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

		protected void Add(Command cmd) => Commands.Add(cmd);

		public abstract void Build();
	}

	public class StoreInstruction(LocationRange loc, StorageValue dest, Value val) : Instruction(loc)
	{
		public readonly StorageValue Dest = dest;
		public readonly Value Value = val;

		public override void Build()
		{
			Add(Value.WriteTo(new DataCommand.Modify(Dest.Storage, Dest.Path).Set()));
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
}
