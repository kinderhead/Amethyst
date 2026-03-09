using Geode;
using Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.GC
{
	public interface IGCBehavior
	{
		public bool TryMark(IValueLike val, FunctionContext ctx);
		public bool CanMark(TypeSpecifier type);
	}

	public abstract class GCBehavior<T> : IGCBehavior where T : TypeSpecifier
	{
		public abstract void Mark(T type, ValueRef val, FunctionContext ctx);
		public abstract bool CanMark(T type);

		public bool CanMark(TypeSpecifier type)
		{
			if (type is T t)
			{
				return CanMark(t);
			}

			return false;
		}

		public bool TryMark(IValueLike val, FunctionContext ctx)
		{
			if (val.Type is T type && CanMark(type))
			{
				Mark(type, val.ToValueRef(), ctx);
				return true;
			}

			return false;
		}
	}
}
