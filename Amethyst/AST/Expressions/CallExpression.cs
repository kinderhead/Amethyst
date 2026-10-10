using Amethyst.IR;
using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Datapack.Net.NumberProviders;
using Geode;
using Geode.Equations;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
    public interface IMethodHolder
    {
        public Expression? GetThis(FunctionContext ctx);
    }

    public class CallExpression(LocationRange loc, Expression func, List<Expression> args) : Expression(loc)
    {
        public readonly List<Expression> Args = args;
        public readonly Expression Function = func;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            return ExecuteImpl(ctx, expected, Function.Execute(ctx, null));
        }

        private ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected, ValueRef function)
        {
            var func = ReferenceType.TryDeref(function, ctx);
            Expression[] newArgs;

            if (Function is IMethodHolder prop && prop.GetThis(ctx) is { } self)
            {
                // Make sure the `this` parameter isn't dereferenced
                newArgs = [self, .. Args];
            }
            else newArgs = [.. Args];

            if (func.Value is Intrinsic i) return i.CallBehavior(ctx, [.. newArgs.Select(i => i.Execute(ctx, null))]);

            ValueRef[]? args = null;

            if (func.Value is OverloadedFunctionValue overload)
            {
                args = [.. newArgs.Select(i => i.Execute(ctx, null))];
                var option = overload.Get(args);
                func = Function is IMethodHolder ? ReferenceType.TryDeref(option, ctx) : new(option);
            }

            if (func.Type is not FunctionType type) throw new InvalidTypeError(func.Type.ToString(), "function");

            args ??= [.. newArgs.Zip(type.Parameters).Select(i => i.First.Execute(ctx, i.Second.Type))];

            if (func.Value is RawFunctionValue f) return f.CallBehavior(ctx, args);

            ctx.Add(new PushFuncArgsInsn(type, ctx.PrepArgs(type, args)));
            return ctx.Add(new DynCallInsn(func));
        }

        protected override Equation ComputeImpl(FunctionContext ctx)
        {
            var func = Function.Execute(ctx, null);

            if (func.Value is Intrinsic i && i.FuncType.Parameters.All(p => p.Type.EffectiveNumberType is not null))
            {
                Expression[] newArgs;

                if (Function is IMethodHolder prop && prop.GetThis(ctx) is { } self)
                {
                    newArgs = [self, .. Args];
                }
                else newArgs = [.. Args];

                return i.Compute(ctx, [.. newArgs.Select(i => i.Compute(ctx))]);
            }


            var value = ExecuteImpl(ctx, null, func);
            var equation = new ValueRefEquation(value);

            return equation.Type == ProviderNumberType.Int
                ? new ValueRefEquation(ctx.ImplicitCast(value, PrimitiveType.Int))
                : new ValueRefEquation(ctx.ImplicitCast(value, PrimitiveType.Float));
        }
    }
}