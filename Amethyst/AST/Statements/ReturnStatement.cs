using Amethyst.AST.Expressions;
using Amethyst.Codegen.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class ReturnStatement(LocationRange loc, Expression? expr) : Statement(loc)
	{
		public readonly Expression? Expression = expr;
		public override IEnumerable<Statement> Statements => [];

		protected override void _Compile(FunctionContext ctx)
		{
			if (Expression is null) ctx.Add(new ExitFrameInstruction(Location));
			else ctx.Add(new ReturnInstruction(Location, Expression.Cast(ctx.FunctionType.ReturnType).Execute(ctx)));
		}
	}
}
