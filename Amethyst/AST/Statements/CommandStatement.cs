using Amethyst.Codegen.IR;
using Datapack.Net.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class CommandStatement(LocationRange loc, string cmd) : Statement(loc)
	{
		public readonly string Command = cmd;

		protected override void _Compile(FunctionContext ctx)
		{
			ctx.Add(new RawCommandInstruction(Location, Command));
		}
	}
}
