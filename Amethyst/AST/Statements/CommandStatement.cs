using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Datapack.Net.Data;
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
				var values = new Dictionary<string, Value>();
				foreach (Match i in matches)
				{
					values[i.Groups[1].Value] = ctx.GetVariable(i.Groups[1].Value);
				}

				if (values.Any(i => i.Value is not MacroValue))
				{
					var func = new MCFunction(ctx.NewInternalID());
					func.Add(new RawCommand(Command, true));
					ctx.TotalFunctions.Add(func);

					var args = ctx.AllocTemp(new PrimitiveTypeSpecifier(NBTType.Compound));
					ctx.Add(new PopulateInstruction(Location, args, values));
					ctx.Add(new CallMacroInstruction(Location, func.ID, args));
				}
				else ctx.Add(new RawCommandInstruction(Location, Command, true));
			}
			else ctx.Add(new RawCommandInstruction(Location, Command));
		}
	}
}
