using System;
using System.Collections.Generic;
using System.Text;

namespace Geode.Util.Algorithm
{
	public static class PostorderTraverser
	{
		extension<T>(Graph<T> tree) where T : GraphNode<T>
		{
			public IEnumerable<T> PostorderTraversal()
			{
				var visited = new HashSet<T>();

				foreach (var i in TraverseNode(tree.Start, visited))
				{
					yield return i;
				}
			}

			public Dictionary<T, int> PostorderIndices()
			{
				var indices = new Dictionary<T, int>();
				int idex = 0;

				foreach (var i in tree.PostorderTraversal())
				{
					indices[i] = idex++;
				}

				return indices;

			}
		}

		private static IEnumerable<T> TraverseNode<T>(T node, HashSet<T> visited) where T : GraphNode<T>
		{
			if (!visited.Contains(node))
			{
				foreach (var i in node.Next)
				{
					foreach (var e in TraverseNode(i, visited))
					{
						yield return e;
					}
				}

				yield return node;
			}
		}
	}
}
