using System.Collections;

namespace Geode.Util
{
    public abstract class GenericTree<T> : IEnumerable<T> where T : GenericTreeNode<T>
    {
        public abstract T Start { get; }

		public IEnumerator<T> GetEnumerator() => new ReversePostorderTraverser<T>(this);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

    public abstract class GenericTreeNode<TSelf> where TSelf : GenericTreeNode<TSelf>
    {
        public readonly HashSet<TSelf> Previous = [];
        public readonly HashSet<TSelf> Next = [];
    }

	public class ReversePostorderTraverser<T>(GenericTree<T> tree) : IEnumerator<T> where T : GenericTreeNode<T>
	{
        public readonly GenericTree<T> Tree = tree;

        private HashSet<T> visited = [];
        private IEnumerator<T>? children;

		public T Current { get; private set; }

		object IEnumerator.Current => Current;

		public void Dispose() { }

		public bool MoveNext()
        {
            if (children is null)
            {
                Current = Tree.Start;
                children = Current.Next.GetEnumerator();
            }

            throw new NotImplementedException();
        }

		public void Reset()
        {
            visited = [];
            children = null;
        }
	}
}
