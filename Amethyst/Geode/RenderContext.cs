using Amethyst.Geode.IR;
using Datapack.Net.Function;

namespace Amethyst.Geode
{
    public record class RenderContext(MCFunction MCFunction, Block Block, GeodeBuilder Builder, FunctionContext Func)
    {
        public void Add(params IEnumerable<Command> cmds) => MCFunction.Add(cmds);
    }
}
