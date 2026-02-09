using Amethyst.Errors;
using Amethyst.IR;
using Amethyst.IR.Instructions;
using Geode;
using Geode.IR;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
	public class IndexExpression(LocationRange loc, Expression val, Expression index) : Expression(loc), IPropertyLikeExpression
	{
		public readonly Expression Value = val;
		public readonly Expression Index = index;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var val = Value.Execute(ctx, null);

			if (val.IsTypeOrRef<ListType>())
			{
				return ctx.Add(new IndexInsn(val, Index.Execute(ctx, PrimitiveType.Int)));
			}
			else if (val.IsTypeOrRef<SimpleMapType>(out var map))
			{
				return ctx.Add(new PropertyInsn(val, Index.Execute(ctx, new UnsafeStringType()), map.Inner));
			}

			throw new CannotIndexError(val.Type.ToString());
		}
	}
}
