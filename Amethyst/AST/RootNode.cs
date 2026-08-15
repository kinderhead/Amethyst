using System.Diagnostics;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.IR;

namespace Amethyst.AST
{
    public class RootNode(LocationRange loc, Compiler ctx) : Node(loc)
    {
        public readonly List<IRootChild> Children = [];
        public readonly Compiler Ctx = ctx;
        public readonly List<FunctionNode> Functions = [];

        public bool BuildSoftTypes()
        {
            var success = true;

            foreach (var i in Children)
            {
                if (!Ctx.WrapError(i.Location, () => Ctx.TypeHandler.RegisterSoftType(i))) success = false;
            }

            return success;
        }

        public bool BuildSymbols()
        {
            var success = true;

            foreach (var i in Children)
            {
                if (!Ctx.WrapError(i.Location, [DebuggerNonUserCode]() => { i.Process(Ctx, this); })) success = false;
            }

            return success;
        }

        public bool CompileFunctions(out List<FunctionContext> funcs)
        {
            var success = true;
            funcs = [];

            foreach (var i in Functions)
            {
                try
                {
                    if (!i.Compile(Ctx, out var ctx) || ctx is null) success = false;
                    else funcs.Add(ctx);
                }
                catch (GeodeError e)
                {
                    e.Display(Ctx, i.Location);
                    success = false;
                }
            }

            return success;
        }
    }

    public interface IRootChild
    {
        LocationRange Location { get; }
        NamespacedID? Provides { get; }

        IEnumerable<NamespacedID> GetTypeDependencies(Compiler ctx);
        void Process(Compiler ctx, RootNode root);
        void SecondPass(Compiler ctx, RootNode root);
    }
}