using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
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
		Negate,
		Reference,
		WeakReference,
		Dereference
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

					var inc = ctx.Add(new AddInsn(ctx.AddLoad(val), new LiteralValue(1)));
					ctx.Add(new StoreInsn(val, inc));
					return val;
				case UnaryOperation.Decrement:
					if (val.Type != PrimitiveType.Int)
					{
						throw new InvalidTypeError(val.Type.ToString(), "int");
					}

					var dec = ctx.Add(new SubInsn(ctx.AddLoad(val), new LiteralValue(1)));
					ctx.Add(new StoreInsn(val, dec));
					return val;
				case UnaryOperation.Negate:
					return ctx.Add(new MulInsn(ctx.AddLoad(ctx.ImplicitCast(val, PrimitiveType.Int)), new LiteralValue(-1)));
				case UnaryOperation.Reference:
					return ctx.ImplicitCast(val, new ReferenceType(val.Type));
				case UnaryOperation.WeakReference:
					return ctx.ImplicitCast(val, new WeakReferenceType(val.Type));
				case UnaryOperation.Dereference:
					if (val.Type is not ReferenceType rt)
					{
						throw new InvalidTypeError(val.Type.ToString(), "reference");
					}

					return rt.Deref(val, ctx);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
