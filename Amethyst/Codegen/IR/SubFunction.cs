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

		public Command Call(Execute baseCmd)
		{
			Context.RequireCompiled();
			var commands = Context.Commands;
			if (commands.Count == 1 && !Context.Ctx.Compiler.Options.AllowOneLiners)
			{
				Context.Ctx.Compiler.Unregister(Func);
				return baseCmd.Run(commands.First());
			}

			if (Context.HasInstruction<ExitFrameInstruction>())
			{
				return baseCmd.If.Function(Func).Run(new ReturnCommand(1));
			}
			else return baseCmd.Run(new FunctionCommand(Func));
		}
	}
}
