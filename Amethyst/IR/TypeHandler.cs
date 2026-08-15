using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR
{
    public class TypeHandler(Compiler compiler)
    {
        public readonly Compiler Compiler = compiler;

        private readonly Dictionary<NamespacedID, IRootChild> softTypes = [];
        private readonly Dictionary<NamespacedID, StructType> typeInfo = [];
        private readonly Dictionary<NamespacedID, GlobalTypeSymbol> types = [];

        public GlobalTypeSymbol this[NamespacedID id] => types[id];

        public void RegisterSoftType(IRootChild node)
        {
            if (node.Provides is not { } id) return;
            if (softTypes.TryGetValue(id, out var old)) throw new RedefinedSymbolError(id.ToString(), old.Location);
            softTypes[id] = node;
        }

        public void Register(TypeSpecifier type)
        {
            if (types.TryGetValue(type.ID, out var old)) throw new RedefinedSymbolError(type.ID.ToString(), old.Location);
            types[type.ID] = new(type.ID, LocationRange.None, type);
        }

        public void Register(GlobalTypeSymbol sym)
        {
            if (types.TryGetValue(sym.ID, out var old)) throw new RedefinedSymbolError(sym.ID.ToString(), old.Location);
            types[sym.ID] = sym;
        }

        public void Remove(NamespacedID id)
        {
            softTypes.Remove(id);
            typeInfo.Remove(id);
            types.Remove(id);
        }

        public TypeSpecifier Find(string baseNamespace, string type)
        {
            if (type.Contains(':'))
            {
                if (types.TryGetValue(type, out var ret)) return ret.Type;
            }
            else if (GeodeBuilder.NamespaceWalk(baseNamespace, type, types) is { } sym) return sym.Type;
            else if (types.TryGetValue($"minecraft:{type}", out var mc)) return mc.Type;
            else if (types.TryGetValue($"builtin:{type}", out var bt)) return bt.Type;

            throw new UnknownTypeError(type);
        }

        public NamespacedID FindSoft(string baseNamespace, string type)
        {
            if (type.Contains(':'))
            {
                if (softTypes.ContainsKey(type)) return type;
            }
            else if (GeodeBuilder.NamespaceWalk(baseNamespace, type, softTypes) is { } sym) return sym.Provides!.Value; // Probably not good but idc
            else if (softTypes.ContainsKey($"minecraft:{type}")) return $"minecraft:{type}";
            else if (softTypes.ContainsKey($"builtin:{type}")) return $"builtin:{type}";

            return Find(baseNamespace, type).ID;
        }

        public void RegisterTypeInfo(StructType type) => typeInfo[type.ID] = type;

        public void GenerateTypeInfo(FunctionContext ctx)
        {
            var info = new ValueRef(ctx.GetVariable("amethyst:type_info"));

            foreach (var (id, val) in typeInfo)
            {
                ctx.Add(new StoreRefInsn(ctx.Add(new PropertyInsn(info, new LiteralValue(id.ToString()), PrimitiveType.Compound)), new LiteralValue(val.GetTypeInfo())));
            }
        }

        public bool GenerateSymbols()
        {
            var errored = false;

            var graph = new TypeGraph();
            foreach (var root in Compiler.Roots.Values)
            {
                foreach (var i in root.Children.Where(i => i.Provides is not null))
                {
                    graph.Add(i, root);
                }
            }

            if (graph.Compute(Compiler)) errored = true;

            HashSet<IRootChild> processed = [];
            foreach (var i in graph.Roots)
            {
                RecurseTypes(i, processed, ref errored);
            }

            if (graph.Nodes.Count != processed.Count)
            {
                errored = true;

                foreach (var i in graph.Nodes)
                {
                    if (!processed.Contains(i.Child) && i.Child.Provides is { } id) new CircularDependencyError(id.ToString()).Display(Compiler, i.Child.Location);
                }
            }

            // Process functions and other things that don't provide types
            foreach (var root in Compiler.Roots.Values)
            {
                foreach (var i in root.Children)
                {
                    if (!Compiler.WrapError(i.Location, () =>
                        {
                            if (!processed.Contains(i)) i.Process(Compiler, root);
                            i.SecondPass(Compiler, root);
                        })) errored = true;
                }
            }

            return !errored;
        }

        private void RecurseTypes(TypeGraphNode node, HashSet<IRootChild> processed, ref bool errored)
        {
            if (!processed.Add(node.Child))
            {
                new CircularDependencyError(node.Child.Provides?.ToString() ?? "<error>").Display(Compiler, node.Child.Location);
                errored = true;
                return;
            }

            if (!Compiler.WrapError(node.Child.Location, () => node.Child.Process(Compiler, node.Root))) errored = true;

            foreach (var i in node.Next)
            {
                RecurseTypes(i, processed, ref errored);
            }
        }
    }

    public record GlobalTypeSymbol(NamespacedID ID, LocationRange Location, TypeSpecifier Type);
}