namespace Geode.Util
{
    public class Graph<T> where T : GraphNode<T>
    {
        public T Start = null!;
    }

    public class UnconnectedGraph<T> where T : GraphNode<T>
    {
        private readonly List<T> nodes = [];
        public IReadOnlyList<T> Nodes => nodes;

        // Probably an actual way to do this
        public IEnumerable<T> Roots => nodes.Where(i => i.Previous.Count == 0);

        public void Add(T node) => nodes.Add(node);
    }

    public class GraphNode<TSelf> where TSelf : GraphNode<TSelf>
    {
        public readonly HashSet<TSelf> Next = [];
        public readonly HashSet<TSelf> Previous = [];

        public void LinkNext(TSelf next)
        {
            Next.Add(next);
            next.Previous.Add((TSelf)this);
        }
    }
}