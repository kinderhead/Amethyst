using System.Diagnostics;
using Amethyst.IR.Types;
using Datapack.Net.NumberProviders;
using Geode;
using Geode.Equations;
using Geode.Errors;
using Geode.IR;
using Geode.Types;

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

        public virtual ValueRef ReferenceHandler(ValueRef val, ReferenceType type, FunctionContext ctx) => ctx.ImplicitCast(val, type);

        protected abstract ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected);

        protected virtual Equation ComputeImpl(FunctionContext ctx)
        {
            var val = Execute(ctx, null);
            var eq = new ValueRefEquation(val);

            // Make sure casting is done
            return eq.Type == ProviderNumberType.Int
                ? new ValueRefEquation(ctx.ImplicitCast(val, PrimitiveType.Int))
                : new ValueRefEquation(ctx.ImplicitCast(val, PrimitiveType.Float));
        }
    }
}