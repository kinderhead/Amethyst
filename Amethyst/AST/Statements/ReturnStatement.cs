using Amethyst.AST.Expressions;
using Amethyst.Errors;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

namespace Amethyst.AST.Statements
{
	public class ReturnStatement(LocationRange loc, Expression? expr) : Statement(loc)
	{
		public readonly Expression? Expression = expr;
		public override IEnumerable<Statement> SubStatements => [];

		public override void Compile(FunctionContext ctx)
		{
			var retType = ctx.Decl.FuncType.ReturnType;
			if (Expression is not null && retType is VoidTypeSpecifier) throw new InvalidTypeError(Expression.ComputeType(ctx).ToString(), "void");

			if (Expression is not null) ctx.Add(new ReturnInsn(Expression.Execute(ctx)));
			else ctx.Add(new ReturnInsn());
		}
	}
}
