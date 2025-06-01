using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public abstract class Expression(LocationRange loc) : Node(loc)
	{
		public TypeSpecifier ComputeType(FunctionContext ctx) => LocatorGuard(ctx, () => _ComputeType(ctx));
		public Value Execute(FunctionContext ctx) => LocatorGuard(ctx, () => _Execute(ctx));
		public void Store(FunctionContext ctx, MutableValue dest) => LocatorGuard(ctx, () => _Store(ctx, dest));

		public void LocatorGuard(FunctionContext ctx, Action cb) => LocatorGuard(ctx, () =>
		{
			cb();
			return 0;
		});

		public T LocatorGuard<T>(FunctionContext ctx, Func<T> cb)
		{
			ctx.PushLocator(this);
			try
			{
				return cb();
			}
			catch (Exception) // Saw somewhere that it might be necessary to explicitly catch all errors so finally is run
			{
				throw;
			}
			finally
			{
				ctx.PopLocator();
			}
		}

		public virtual Expression Cast(TypeSpecifier type) => new CastExpression(Location, type, this);

		protected abstract TypeSpecifier _ComputeType(FunctionContext ctx);
		protected abstract Value _Execute(FunctionContext ctx);
		protected virtual void _Store(FunctionContext ctx, MutableValue dest)
		{
			dest.Store(ctx, Execute(ctx));
		}
	}

	public class LiteralExpression(LocationRange loc, NBTValue val) : Expression(loc)
	{
		public readonly NBTValue Value = val;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(Value.Type);
		protected override Value _Execute(FunctionContext ctx) => new LiteralValue(Value);
	}

	public class VariableExpression(LocationRange loc, string name) : Expression(loc)
	{
		public readonly string Name = name;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => ctx.GetVariable(Name).Type;
		protected override Value _Execute(FunctionContext ctx) => ctx.GetVariable(Name);
	}
}
