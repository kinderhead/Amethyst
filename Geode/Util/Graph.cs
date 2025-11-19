using System.Collections;

namespace Geode.Util
{
    public class Graph<T> where T : GraphNode<T>
    {
		public T Start;
	}

    public class GraphNode<TSelf> where TSelf : GraphNode<TSelf>
    {
        public readonly HashSet<TSelf> Previous = [];
        public readonly HashSet<TSelf> Next = [];

		public void LinkNext(TSelf next)
		{
			Next.Add(next);
			next.Previous.Add((TSelf)this);
		}
	}
}
