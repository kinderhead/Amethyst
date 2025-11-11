using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
	public enum UnaryOperation
	{
		Increment,
		Decrement,
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
					if (val.Type != PrimitiveType.Int)
					{
						throw new InvalidTypeError(val.Type.ToString(), "int");
					}

					var inc = ctx.Add(new AddInsn(ctx.Add(new LoadInsn(val)), new LiteralValue(1)));
					ctx.Add(new StoreInsn(val, inc));
					return val;
				case UnaryOperation.Decrement:
					if (val.Type != PrimitiveType.Int)
					{
						throw new InvalidTypeError(val.Type.ToString(), "int");
					}

					var dec = ctx.Add(new SubInsn(val, new LiteralValue(1)));
					ctx.Add(new StoreInsn(val, dec));
					return val;
				case UnaryOperation.Negate:
					return ctx.Add(new MulInsn(ctx.ImplicitCast(val, PrimitiveType.Int), new LiteralValue(-1)));
				default:
					throw new NotImplementedException();
			}
		}
	}
}
