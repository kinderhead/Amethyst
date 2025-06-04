using Amethyst.AST;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen.IR
{
	public class InitFrameInstruction(LocationRange loc) : Instruction(loc)
	{
		public override void Build()
		{
			if (Ctx.Ctx.FunctionType.Parameters.Length == 0) Add(new DataCommand.Modify(new Storage(Compiler.RuntimeID), "stack").Append().Value("{}"));

			foreach (var i in Ctx.Ctx.LocalScores)
			{
				Add(i.ReadTo(new Storage(Compiler.RuntimeID), $"stack[-1].$score{i.Score.Name}"));
			}
		}
	}

	public class ExitFrameInstruction(LocationRange loc) : Instruction(loc)
	{
		public override void Build()
		{
			foreach (var i in Ctx.Ctx.LocalScores)
			{
				Add(new StorageValue(new Storage(Compiler.RuntimeID), $"stack[-1].$score{i.Score.Name}", i.Type).ReadTo(i.Target, i.Score));
			}

			Add(new DataCommand.Remove(new Storage(Compiler.RuntimeID), "stack[-1]"));
		}
	}

	public class ReturnInstruction(LocationRange loc, Value val) : ExitFrameInstruction(loc)
	{
		public readonly Value Value = val;

		public override void Build()
		{
			Add(Value.ReadTo(Compiler.RuntimeID, "return"));
			base.Build();
			Add(new ReturnCommand(1));
		}
	}
}
