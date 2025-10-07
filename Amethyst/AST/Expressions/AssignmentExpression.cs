using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
	public enum AssignmentType
	{
		Normal,
		Addition,
		Subtraction,
		Multiplication,
		Division
	}

	public class AssignmentExpression(LocationRange loc, Expression dest, AssignmentType type, Expression expr) : Expression(loc)
	{
		public readonly Expression Dest = dest;
		public readonly AssignmentType Type = type;
		public readonly Expression Expression = expr;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var dest = Dest.Execute(ctx, null);
			var val = Expression.Execute(ctx, dest.Type.AssignmentOverloadType, false);

			switch (Type)
			{
				case AssignmentType.Addition:
					val = ctx.Add(new AddInsn(ctx.ImplicitCast(dest, PrimitiveTypeSpecifier.Int), val));
					break;
				case AssignmentType.Subtraction:
					val = ctx.Add(new SubInsn(ctx.ImplicitCast(dest, PrimitiveTypeSpecifier.Int), val));
					break;
				case AssignmentType.Multiplication:
					val = ctx.Add(new MulInsn(ctx.ImplicitCast(dest, PrimitiveTypeSpecifier.Int), val));
					break;
				case AssignmentType.Division:
					val = ctx.Add(new DivInsn(ctx.ImplicitCast(dest, PrimitiveTypeSpecifier.Int), val));
					break;
				default:
					break;
			}

			dest.Type.AssignmentOverload(dest, val, ctx);

			return val;
		}
	}
}
