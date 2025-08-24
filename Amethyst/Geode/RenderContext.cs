using Amethyst.Geode.IR;
using Datapack.Net.Function;

namespace Amethyst.Geode
{
    public record class RenderContext(MCFunction MCFunction, Block Block, GeodeBuilder Builder, FunctionContext Func)
    {
        public virtual void Add(params IEnumerable<Command> cmds) => MCFunction.Add(cmds);

        public List<Command> WithFaux(Action<FauxRenderContext> func)
        {
            var ctx = new FauxRenderContext(MCFunction, Block, Builder, Func);
            func(ctx);
            return ctx.Commands;
        }
    }

    public record class FauxRenderContext(MCFunction MCFunction, Block Block, GeodeBuilder Builder, FunctionContext Func) : RenderContext(MCFunction, Block, Builder, Func)
    {
        public readonly List<Command> Commands = [];
        public override void Add(params IEnumerable<Command> cmds) => Commands.AddRange(cmds);
    }
}
