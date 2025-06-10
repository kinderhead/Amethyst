using Amethyst.AST.Expressions;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
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
			if (Expression is null)
			{
				if (ctx.ReturnValue.Type is not VoidTypeSpecifier) throw new InvalidTypeError(Location, "void", ctx.ReturnValue.Type.ToString());
				if (ctx.InlineDepth == 0) ctx.Add(new ExitFrameInstruction(Location));
			}
			else
			{
				if (ctx.ReturnValue.Type is VoidTypeSpecifier) throw new InvalidTypeError(Location, ctx.ReturnValue.Type.ToString(), "void");

				Expression.Cast(ctx.ReturnValue.Type).Store(ctx, ctx.ReturnValue);
				if (ctx.InlineDepth == 0) ctx.Add(new ReturnInstruction(Location));
			}
		}
	}
}
