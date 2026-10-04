using System.Diagnostics;
using Geode;
using Geode.Equations;
using Geode.Errors;
using Geode.IR;

namespace Amethyst.AST
{
    public abstract class Expression(LocationRange loc) : Node(loc)
    {
        public ValueRef Execute(FunctionContext ctx, TypeSpecifier? expected, bool autoCast = true)
        {
            ValueRef? ret = null;
            if (!ctx.Compiler.WrapError(Location, ctx, [DebuggerNonUserCode]() => ret = ExecuteImpl(ctx, expected))) throw new EmptyGeodeError();

            if (expected is not null && autoCast) return ctx.ImplicitCast(ret!, expected);

            return ret!;
        }

        public Equation Compute(FunctionContext ctx)
        {
            Equation? ret = null;
            if (!ctx.Compiler.WrapError(Location, ctx, [DebuggerNonUserCode]() => ret = ComputeImpl(ctx))) throw new EmptyGeodeError();

            return ret!;
        }

        public virtual void ExecuteChain(ExecuteChain chain, FunctionContext ctx, bool invert = false)
        {
            var val = Execute(ctx, null);
            val.Type.ExecuteChainOverload(val, chain, ctx, invert);
        }

        protected abstract ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected);
        protected virtual Equation ComputeImpl(FunctionContext ctx) => new ValueRefEquation(Execute(ctx, null));
    }
}