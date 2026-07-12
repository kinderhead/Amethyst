using Geode;
using Geode.IR;

namespace Amethyst.GC
{
	public interface IGCBehavior
	{
		bool TryMark(IValueLike val, FunctionContext ctx);
		bool CanMark(TypeSpecifier type);
	}

	public abstract class GCBehavior<T> : IGCBehavior where T : TypeSpecifier
	{
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

		public abstract void Mark(T type, ValueRef val, FunctionContext ctx);
		public abstract bool CanMark(T type);
	}
}