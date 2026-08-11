using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR
{
    public static class FunctionContextExtensions
    {
        extension(FunctionContext ctx)
        {
            public ValueRef GetProperty(ValueRef val, string name)
            {
                if (val.Type.HasProperty(name) is { } t) return ctx.Add(new PropertyInsn(val, LiteralValue.Raw(name), t));
                if (ctx.GetMethodOrNull(val, name) is { } method) return method;
                if (val.Type.DefaultPropertyType is { } t2) return ctx.Add(new PropertyInsn(val, LiteralValue.Raw(name), t2));
                throw new PropertyError(val.Type.ToString(), name);
            }

            public ValueRef? GetMethodOrNull(ValueRef val, string name, TypeSpecifier? effectiveMethodType = null)
            {
                effectiveMethodType ??= val.Type is ReferenceType r1 ? r1.Inner : val.Type;
                var global = ctx.GetGlobal($"{effectiveMethodType.ID}/{name}");

                switch (global)
                {
                    case IFunctionLike { FuncType.Parameters.Length: >= 1 } func:
                    {
                        // With references
                        var genericFunc = func.CloneWithType(func.FuncType.ApplyGenericWithParams([new ReferenceType(effectiveMethodType)]));
                        var firstArgType = genericFunc.FuncType.Parameters[0].Type;

                        if (firstArgType is ReferenceType r2 && effectiveMethodType.Implements(r2.Inner)) return new(genericFunc);

                        // Without references
                        genericFunc = func.CloneWithType(func.FuncType.ApplyGenericWithParams([effectiveMethodType]));
                        firstArgType = genericFunc.FuncType.Parameters[0].Type;

                        if (effectiveMethodType.Implements(firstArgType)) return new(genericFunc);

                        break;
                    }
                    case OverloadedFunctionValue:
                        return new(global);
                }

                return effectiveMethodType.BaseClass != effectiveMethodType ? ctx.GetMethodOrNull(val, name, effectiveMethodType.BaseClass) : null;
            }
        }
    }
}