using Amethyst.Geode.IR;
using Datapack.Net.Function;

namespace Amethyst.Geode
{
    public record class RenderContext(MCFunction Function, Block Block, GeodeBuilder Builder)
    {
        public void Add(params IEnumerable<Command> cmds) => Function.Add(cmds);
    }
}
