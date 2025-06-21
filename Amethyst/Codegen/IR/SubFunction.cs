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
	public class SubFunction(FunctionContext.Frame ctx, NamespacedID id, MCFunction func)
	{
		public readonly FunctionContext.Frame Frame = ctx;
		public readonly NamespacedID ID = id;
		public readonly MCFunction Func = func;

		public Command[] Call(Execute baseCmd)
		{
			Frame.RequireCompiled();
			var commands = Frame.Commands;
			if (commands.Count == 1 && !Frame.Ctx.Compiler.Options.AllowOneLiners)
			{
				Frame.Ctx.Compiler.Unregister(Func);
				return [baseCmd.Run(commands.First())];
			}

			NBTCompound marcroArgs = [.. Frame.Ctx.FunctionType.Parameters.Where(i => i.Modifiers.HasFlag(AST.ParameterModifiers.Macro)).Select(i => new KeyValuePair<string, NBTValue>(i.Name, $"$({i.Name})"))];

			FunctionCommand call;
			if (marcroArgs.Count == 0) call = new(Func);
			else call = new(Func, marcroArgs, true);

			if (Frame.HasInstruction<ExitFrameInstruction>())
			{
				if (Frame.Ctx.InlineDepth != 0) throw new NotImplementedException("Return statements in conditional inline functions are not supported yet");

				if (Frame.Ctx.HasMacros)
				{
					var score = Frame.Ctx.Compiler.Score("_fail");
					return [ // Don't make copy of baseCmd because the sub function could modify the effect
						new Scoreboard.Players.Set(Compiler.RuntimeEntity, score, 0),
						baseCmd.Store(Compiler.RuntimeEntity, score).Run(call),
						new Execute().If.Score(Compiler.RuntimeEntity, score, 1).Run(new ReturnCommand(1))
					];
				}
				else return [baseCmd.If.Function(Func).Run(new ReturnCommand(1))];
			}
			else return [baseCmd.Run(call)];
		}
	}
}
