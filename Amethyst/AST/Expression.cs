using Geode;
using Geode.Errors;
using Geode.IR;

namespace Amethyst.AST
{
	public abstract class Expression(LocationRange loc) : Node(loc)
	{
		public ValueRef Execute(FunctionContext ctx, TypeSpecifier? expected, bool autoCast = true)
		{
			ValueRef? ret = null;
			if (!ctx.Compiler.WrapError(Location, ctx, [System.Diagnostics.DebuggerNonUserCode]  () => ret = ExecuteImpl(ctx, expected)))
			{
				throw new EmptyGeodeError();
			}

			if (expected is not null && autoCast)
			{
				return ctx.ImplicitCast(ret!, expected);
			}
			else
			{
				return ret!;
			}
		}

		public virtual void ExecuteChain(ExecuteChain chain, FunctionContext ctx, bool invert = false)
		{
			var val = Execute(ctx, null);
			val.Type.ExecuteChainOverload(val, chain, ctx, invert);
		}

		protected abstract ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected);
	}
}
