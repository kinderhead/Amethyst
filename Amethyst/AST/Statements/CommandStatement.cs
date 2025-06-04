using Amethyst.Codegen.IR;
using Datapack.Net.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class CommandStatement(LocationRange loc, string cmd) : Statement(loc)
	{
		public readonly string Command = cmd;
		public override IEnumerable<Statement> Statements => [];

		protected override void _Compile(FunctionContext ctx)
		{
			var matches = Datapack.Net.Function.Command.MacroRegex.Matches(Command);
			if (matches.Count != 0)
			{
				foreach (Match i in matches)
				{
					var val = ctx.GetVariable(i.Groups[1].Value);
					
				}
			}
			else ctx.Add(new RawCommandInstruction(Location, Command));
		}
	}
}
