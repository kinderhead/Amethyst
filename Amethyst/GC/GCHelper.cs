using Amethyst.GC.Behaviors;
using Geode;
using Geode.IR;

namespace Amethyst.GC
{
	public static class GCHelper
	{
		private static readonly List<IGCBehavior> behaviors = [];

		static GCHelper()
		{
			Register<ReferenceBehavior>();
			Register<ListBehavior>();
		}

		public static IReadOnlyCollection<IGCBehavior> Behaviors => behaviors;

		public static void Mark(IValueLike val, FunctionContext ctx)
		{
			foreach (var i in behaviors)
			{
				if (i.TryMark(val, ctx))
				{
					return;
				}
			}
		}

		public static bool CanMark(TypeSpecifier type)
		{
			foreach (var i in behaviors)
			{
				if (i.CanMark(type))
				{
					return true;
				}
			}

			return false;
		}

		public static void Register<T>() where T : IGCBehavior, new() => behaviors.Add(new T());
	}
}