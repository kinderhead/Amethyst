using Amethyst.AST.Expressions;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class IfStatement(LocationRange loc, Expression expr, Statement stmt, Statement? elseStmt) : Statement(loc)
	{
		public readonly Expression Expression = expr;
		public readonly Statement Statement = stmt;
		public readonly Statement? Else = elseStmt;

		protected override void _Compile(FunctionContext ctx)
		{
			var truthy = new ExecuteWrapper();

			Expression.Compare(ctx, truthy);

			if (Else is not null)
			{
				if (!truthy.NeverExecute)
				{
					var check = ctx.AllocElseScore();
					// Probably a better way to do this
					check.Store(ctx, new LiteralValue(new NBTInt(0)));
					ctx.Add(new StoreIfSuccessInstruction(Location, truthy.Cmd, check, new LiteralValue(new NBTInt(1))));
					ctx.Add(new CompareJumpInstruction(Location, new Execute().Unless.Score(check.Target, check.Score, 0), ctx.SubFunc(Statement)));
					ctx.Add(new CompareJumpInstruction(Location, new Execute().If.Score(check.Target, check.Score, 0), ctx.SubFunc(Else)));
				}
				else Else.Compile(ctx);
			}
			else if (!truthy.NeverExecute) ctx.Add(new CompareJumpInstruction(Location, truthy.Cmd, ctx.SubFunc(Statement)));
		}
	}
}
