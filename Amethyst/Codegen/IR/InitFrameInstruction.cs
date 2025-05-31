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
			Add(new DataCommand.Modify(new Storage(Compiler.RuntimeID), "stack").Append().Value("{}"));
		}
	}

	public class ExitFrameInstruction(LocationRange loc) : Instruction(loc)
	{
		public override void Build()
		{
			Add(new DataCommand.Remove(new Storage(Compiler.RuntimeID), "stack[-1]"));
		}
	}
}
