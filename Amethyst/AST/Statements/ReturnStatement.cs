using Amethyst.AST.Expressions;
using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Statements
{
	public class ReturnStatement(LocationRange loc, Expression? expr) : Statement(loc)
	{
		public readonly Expression? Expression = expr;
		public override IEnumerable<Statement> Statements => [];

		public override void Compile(FunctionContext ctx)
		{
			if (ctx.Decl.FuncType.ReturnType is not VoidTypeSpecifier) throw new NotImplementedException();
			if (Expression is not null) throw new InvalidTypeError(Expression.ComputeType(ctx).ToString(), "void");

			ctx.Add(new ReturnInsn());
		}
	}
}
