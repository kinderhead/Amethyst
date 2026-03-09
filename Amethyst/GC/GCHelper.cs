using Amethyst.GC.Behaviors;
using Geode;
using Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.GC
{
	public static class GCHelper
	{
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

		static GCHelper()
		{
			Register<ReferenceBehavior>();
			Register<ListBehavior>();
		}

		public static void Register<T>() where T : IGCBehavior, new() => behaviors.Add(new T());

		private readonly static List<IGCBehavior> behaviors = [];
		public static IReadOnlyCollection<IGCBehavior> Behaviors => behaviors;
	}
}
