using Datapack.Net.Utils;
using Geode.Util;

namespace Amethyst.AST
{
    public class TypeGraph : UnconnectedGraph<TypeGraphNode>
    {
        private readonly Dictionary<NamespacedID, TypeGraphNode> providers = [];

        public void Add(IRootChild child, RootNode root)
        {
            var node = new TypeGraphNode(child, root);
            Add(node);
            if (child.Provides is { } id) providers[id] = node;
        }

        public bool Compute(Compiler ctx)
        {
            var errored = false;

            foreach (var i in Nodes)
            {
                if (!ctx.WrapError(i.Child.Location, () => i.Child.GetTypeDependencies(ctx).ToArray(), out var deps)) errored = true;
                else
                {
                    foreach (var id in deps)
                    {
                        if (providers.TryGetValue(id, out var node)) node.LinkNext(i);
                    }
                }
            }

            return errored;
        }
    }

    public class TypeGraphNode(IRootChild child, RootNode root) : GraphNode<TypeGraphNode>
    {
        public readonly IRootChild Child = child;
        public readonly RootNode Root = root;
    }
}