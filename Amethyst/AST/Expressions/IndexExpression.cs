using Amethyst.Errors;
using Amethyst.IR;
using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
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
			ValueRef ret;

			if (val.IsTypeOrRef<ListType>())
			{
				ret = ctx.Add(new IndexInsn(val, Index.Execute(ctx, PrimitiveType.Int)));
			}
			else if (val.IsTypeOrRef<SimpleMapType>(out var map))
			{
				ret = ctx.Add(new PropertyInsn(val, Index.Execute(ctx, PrimitiveType.String), map.Inner));
			}
			else if (val.IsTypeOrRef<PrimitiveType>(out var raw) && raw == PrimitiveType.Compound)
			{
				ret = ctx.Add(new PropertyInsn(val, Index.Execute(ctx, PrimitiveType.String), raw));
			}
			else
			{
				throw new CannotIndexError(val.Type.ToString());
			}

			if (expected is null)
			{
				ret = ctx.ImplicitCast(ret, ((ReferenceType)ret.Type).Inner);
			}

			return ret;
		}
	}
}
