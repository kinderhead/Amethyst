using Amethyst.Codegen.IR;
using Amethyst.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class BlockNode(LocationRange loc) : Statement(loc)
	{
		private readonly List<Statement> statements = [];
		public override IEnumerable<Statement> Statements => statements;

		protected override void _Compile(FunctionContext ctx)
		{
			if (!CompileWithErrorChecking(ctx)) throw new EmptyAmethystError();
		}

		public void Add(Statement stmt) => statements.Add(stmt);

		public bool CompileWithErrorChecking(FunctionContext ctx)
		{
			var success = true;

			foreach (var i in Statements)
			{
				if (!ctx.Compiler.WrapError(() => i.Compile(ctx))) success = false;
				if (ctx.CurrentFrame.Instructions.Last() is ExitFrameInstruction) break;
			}

			return success;
		}
	}
}
