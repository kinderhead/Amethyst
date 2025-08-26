using Amethyst.AST.Expressions;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Statements
{
	public class InitAssignmentNode(LocationRange loc, AbstractTypeSpecifier type, string name, Expression? expr) : Statement(loc)
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly Expression? Expression = expr;
		public override IEnumerable<Statement> Statements => [];

		public override void Compile(FunctionContext ctx)
		{
			var type = Type.Resolve(ctx, Expression is not null);
			if (type is VarTypeSpecifier && Expression is not null) type = Expression.ComputeType(ctx);
			var val = ctx.RegisterLocal(Name, type);

			if (Expression is not null)
			{
				ctx.Add(new StoreInsn(val, ctx.ImplicitCast(Expression.Execute(ctx), val.Type)));
			}
		}
	}
}
