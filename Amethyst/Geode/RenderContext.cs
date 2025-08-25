using Amethyst.Geode.IR;
using Datapack.Net.Function;

namespace Amethyst.Geode
{
    public record class RenderContext(MCFunction MCFunction, Block Block, GeodeBuilder Builder, FunctionContext Func)
    {
        public virtual void Add(params IEnumerable<Command> cmds) => MCFunction.Add(cmds);

        public void StoreCompound(StorageValue dest, Dictionary<string, ValueRef> dict)
        {
            var nbt = new Datapack.Net.Data.NBTCompound();
            var runtime = new Dictionary<string, Value>();

            foreach (var (key, val) in dict.Select(i => (i.Key, i.Value.Expect())))
            {
                if (val.IsLiteral) nbt[key] = ((LiteralValue)val).Value;
                else runtime[key] = val;
            }

            dest.Store(new LiteralValue(nbt), this);

            foreach (var (key, value) in runtime)
            {
                dest.Property(key, value.Type).Store(value, this);
            }
        }

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
