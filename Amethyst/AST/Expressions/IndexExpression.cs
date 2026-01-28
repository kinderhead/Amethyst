using Amethyst.IR.Instructions;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
	public class IndexExpression(LocationRange loc, Expression list, Expression index) : Expression(loc), IPropertyLikeExpression
	{
		public readonly Expression List = list;
		public readonly Expression Index = index;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected) => ctx.Add(new IndexInsn(List.Execute(ctx, null), Index.Execute(ctx, PrimitiveType.Int)));
	}
}
