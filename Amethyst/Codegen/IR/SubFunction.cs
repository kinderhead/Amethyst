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
	public class SubFunction(FunctionContext.Frame ctx, NamespacedID id, MCFunction func)
	{
		public readonly FunctionContext.Frame Context = ctx;
		public readonly NamespacedID ID = id;
		public readonly MCFunction Func = func;

		public Command Call()
		{
			Context.RequireCompiled();
			var commands = Context.Commands;
			if (commands.Count == 1 && !Context.Ctx.Compiler.Options.AllowOneLiners)
			{
				Context.Ctx.Compiler.Unregister(Func);
				return commands.First();
			}
			else return new FunctionCommand(Func);
		}
	}
}
