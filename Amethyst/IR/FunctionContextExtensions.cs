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
				if (ctx.GetMethodOrNull(val, name) is ValueRef method)
				{
					return method;
				}
				else if (val.Type.Property(name) is TypeSpecifier t)
				{
					return ctx.Add(new PropertyInsn(val, new LiteralValue(name), t));
				}

				throw new PropertyError(val.Type.ToString(), name);
			}

			public ValueRef? GetMethodOrNull(ValueRef val, string name, TypeSpecifier? effectiveMethodType = null)
			{
				effectiveMethodType ??= val.Type is ReferenceTypeSpecifier r1 ? r1.Inner : val.Type;
				if (ctx.GetGlobal($"{effectiveMethodType.ID}/{name}") is IFunctionLike func && func.FuncType.Parameters.Length >= 1)
				{
					func = func.CloneWithType(func.FuncType.ApplyGenericWithParams([new ReferenceTypeSpecifier(effectiveMethodType)]));
					var firstArgType = func.FuncType.Parameters[0].Type;
					if (firstArgType is ReferenceTypeSpecifier r2 && effectiveMethodType.Implements(r2.Inner))
					{
						return func.AsMethod(val, ctx);
					}
				}

				if (effectiveMethodType.BaseClass != effectiveMethodType)
				{
					return ctx.GetMethodOrNull(val, name, effectiveMethodType.BaseClass);
				}
				else
				{
					return null;
				}
			}
		}
	}
}
