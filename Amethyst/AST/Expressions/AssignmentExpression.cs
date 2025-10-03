using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

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

			if (Type != AssignmentType.Normal && val.Type != PrimitiveTypeSpecifier.Int)
			{
				throw new InvalidTypeError(val.Type.ToString(), "int");
			}

			switch (Type)
			{
				case AssignmentType.Addition:
					val = ctx.Add(new AddInsn(dest, val));
					break;
				case AssignmentType.Subtraction:
					val = ctx.Add(new SubInsn(dest, val));
					break;
				case AssignmentType.Multiplication:
					val = ctx.Add(new MulInsn(dest, val));
					break;
				case AssignmentType.Division:
					val = ctx.Add(new DivInsn(dest, val));
					break;
				default:
					break;
			}

			dest.Type.AssignmentOverload(dest, val, ctx);

			return val;
		}
	}
}
