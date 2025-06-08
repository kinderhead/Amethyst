using Amethyst.AST;
using Amethyst.Errors;
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
			if (Ctx.Ctx.Node.Tags.Count != 0 && Ctx.Ctx.FunctionType.Parameters.Length != 0) throw new TagError(Ctx.Ctx.Node.Location, "functions with arguments cannot have tags");

			if (Ctx.Ctx.UsesStack && !Ctx.Ctx.FunctionType.Parameters.Any(i => i.Modifiers.HasFlag(ParameterModifiers.Macro)))
			{
				Add(new DataCommand.Modify(new Storage(Compiler.RuntimeID), "stack").Append().Value("{}"));
			}

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

			if (Ctx.Ctx.UsesStack)
			{
				if ((Ctx.Ctx.Node.Modifiers & FunctionModifiers.NoStack) == FunctionModifiers.NoStack) throw new ModifierError(Location, "function uses stack when it is not supposed to");
				Add(new DataCommand.Remove(new Storage(Compiler.RuntimeID), "stack[-1]"));
			}
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
