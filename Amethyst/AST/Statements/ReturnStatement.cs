using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Statements
{
	public class ReturnStatement(LocationRange loc, Expression? expr) : Statement(loc)
	{
		public readonly Expression? Expression = expr;

		public override void Compile(FunctionContext ctx)
		{
			var retType = ctx.Decl.FuncType.ReturnType;
			var val = Expression?.Execute(ctx, retType);

			if (val is not null && retType is VoidType)
			{
				throw new InvalidTypeError(val.Type.ToString(), "void");
			}
			else if (val is not null)
			{
				ctx.Add(new ReturnInsn(val));
			}
			else
			{
				ctx.Add(new ReturnInsn());
			}
		}
	}
}
