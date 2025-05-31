using Amethyst.Codegen.IR;
using Amethyst.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class BlockNode(LocationRange loc) : AbstractStatement(loc)
	{
		public readonly List<AbstractStatement> Statements = [];

		protected override void _Compile(FunctionContext ctx)
		{
			if (!CompileWithErrorChecking(ctx)) throw new EmptyAmethystError();
		}

		public bool CompileWithErrorChecking(FunctionContext ctx)
		{
			var success = true;

			foreach (var i in Statements)
			{
				if (!ctx.Compiler.WrapError(() => i.Compile(ctx))) success = false;
			}

			return success;
		}
	}
}
