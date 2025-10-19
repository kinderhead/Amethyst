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
				if (val.Type.Property(name) is TypeSpecifier t)
				{
					return ctx.Add(new PropertyInsn(val, new LiteralValue(name), t));
				}
				else if (ctx.GetMethodOrNull(val, name) is ValueRef method)
				{
					return method;
				}

				throw new PropertyError(val.Type.ToString(), name);
			}

			public ValueRef? GetMethodOrNull(ValueRef val, string name, TypeSpecifier? effectiveMethodType = null)
			{
				effectiveMethodType ??= val.Type is ReferenceType r1 ? r1.Inner : val.Type;
				if (ctx.GetGlobal($"{effectiveMethodType.ID}/{name}") is IFunctionLike func && func.FuncType.Parameters.Length >= 1)
				{
					func = func.CloneWithType(func.FuncType.ApplyGenericWithParams([new ReferenceType(effectiveMethodType)]));
					var firstArgType = func.FuncType.Parameters[0].Type;
					if (firstArgType is ReferenceType r2 && effectiveMethodType.Implements(r2.Inner))
					{
						return new(func);
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
