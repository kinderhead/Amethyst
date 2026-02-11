using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Datapack.Net.Data;
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
				if (val.Type.HasProperty(name) is TypeSpecifier t)
				{
					return ctx.Add(new PropertyInsn(val, new LiteralValue(new NBTRawString(name)), t));
				}
				else if (ctx.GetMethodOrNull(val, name) is ValueRef method)
				{
					return method;
				}
				else if (val.Type.DefaultPropertyType is TypeSpecifier t2)
				{
					return ctx.Add(new PropertyInsn(val, new LiteralValue(new NBTRawString(name)), t2));
				}

				throw new PropertyError(val.Type.ToString(), name);
			}

			public ValueRef? GetMethodOrNull(ValueRef val, string name, TypeSpecifier? effectiveMethodType = null)
			{
				effectiveMethodType ??= val.Type is ReferenceType r1 ? r1.Inner : val.Type;
				var global = ctx.GetGlobal($"{effectiveMethodType.ID}/{name}");

				if (global is IFunctionLike func && func.FuncType.Parameters.Length >= 1)
				{
					// With references
					var genericFunc = func.CloneWithType(func.FuncType.ApplyGenericWithParams([new ReferenceType(effectiveMethodType)]));
					var firstArgType = genericFunc.FuncType.Parameters[0].Type;

					if (firstArgType is ReferenceType r2 && effectiveMethodType.Implements(r2.Inner))
					{
						return new(genericFunc);
					}

					// Without references
					genericFunc = func.CloneWithType(func.FuncType.ApplyGenericWithParams([effectiveMethodType]));
					firstArgType = genericFunc.FuncType.Parameters[0].Type;

					if (effectiveMethodType.Implements(firstArgType))
					{
						return new(genericFunc);
					}
				}
				else if (global is OverloadedFunctionValue)
				{
					return new(global);
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
