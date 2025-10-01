using Amethyst.AST.Expressions;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

namespace Amethyst.AST.Statements
{
	public class InitAssignmentNode(LocationRange loc, AbstractTypeSpecifier type, string name, Expression? expr) : Statement(loc)
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly Expression? Expression = expr;
		public override IEnumerable<Statement> SubStatements => [];

		public override void Compile(FunctionContext ctx)
		{
			var type = Type.Resolve(ctx, Expression is not null);
			var val = Expression is null ? type.DefaultValue : Expression.Execute(ctx);
			if (type is VarTypeSpecifier && Expression is not null) type = val.Type;
			var dest = ctx.RegisterLocal(Name, type);

			ctx.Add(new StoreInsn(dest, ctx.ImplicitCast(val, type), false));
		}
	}
}
