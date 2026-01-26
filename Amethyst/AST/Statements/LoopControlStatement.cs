using Geode;
using Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Statements
{
	public enum LoopControlType
	{
		Continue,
		Break
	}

	public class LoopControlStatement(LocationRange loc, LoopControlType type) : Statement(loc)
	{
		public readonly LoopControlType Type = type;

		public override void Compile(FunctionContext ctx)
		{
			if (Type == LoopControlType.Continue)
			{
				ctx.LoopContinue();
			}
			else
			{
				ctx.LoopBreak();
			}
		}
	}
}
