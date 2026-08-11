using Amethyst.IR;
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
    public interface IMethodHolder
    {
        public Expression GetThis(FunctionContext ctx);
    }

    public class CallExpression(LocationRange loc, Expression func, List<Expression> args) : Expression(loc)
    {
        public readonly List<Expression> Args = args;
        public readonly Expression Function = func;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            var func = ReferenceType.TryDeref(Function.Execute(ctx, null), ctx);
            Expression[] newArgs;

            if (Function is IMethodHolder prop)
            {
                // Make sure the `this` parameter isn't dereferenced
                newArgs = [prop.GetThis(ctx), .. Args];
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
    }
}