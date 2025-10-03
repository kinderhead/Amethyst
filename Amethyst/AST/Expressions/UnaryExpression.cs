using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;

namespace Amethyst.AST.Expressions
{
	public enum UnaryOperation
	{
		Increment,
		Decrement,
		Not,
		Negate
	}

	public class UnaryExpression(LocationRange loc, UnaryOperation op, Expression val) : Expression(loc)
	{
		public readonly UnaryOperation Op = op;
		public readonly Expression Value = val;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var val = Value.Execute(ctx, null);

			switch (Op)
			{
				case UnaryOperation.Increment:
					if (val.Type != PrimitiveTypeSpecifier.Int)
					{
						throw new InvalidTypeError(val.Type.ToString(), "int");
					}

					var inc = ctx.Add(new AddInsn(val, new LiteralValue(1)));
					ctx.Add(new StoreInsn(val, inc));
					return val;
				case UnaryOperation.Decrement:
					if (val.Type != PrimitiveTypeSpecifier.Int)
					{
						throw new InvalidTypeError(val.Type.ToString(), "int");
					}

					var dec = ctx.Add(new SubInsn(val, new LiteralValue(1)));
					ctx.Add(new StoreInsn(val, dec));
					return val;
				case UnaryOperation.Not:
					return ctx.Add(new NotInsn(val));
				case UnaryOperation.Negate:
					return ctx.Add(new MulInsn(ctx.ImplicitCast(val, PrimitiveTypeSpecifier.Int), new LiteralValue(-1)));
				default:
					throw new NotImplementedException();
			}
		}
	}
}
