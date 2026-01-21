namespace Geode.IR
{
    public class PhiContext
    {
        private readonly Dictionary<Block, List<(ValueRef src, ValueRef dest)>> map = [];

        public IEnumerable<ValueRef> Dependencies => map.Values.SelectMany(i => i.Select(i => i.src));

        public void Map(Block block, ValueRef src, ValueRef dest)
        {
            if (!map.TryGetValue(block, out var vars))
            {
                vars = [];
                map[block] = vars;
            }

            vars.Add((src, dest));
        }

        public void JumpToBlockCommands(Block dest, RenderContext ctx)
        {
            if (map.TryGetValue(dest, out var vars))
            {
                foreach (var i in vars)
                {
                    var s = i.src.Expect();
                    var d = i.dest.Expect<LValue>();

                    if (!s.Equals(d))
                    {
                        d.Store(s, ctx);
                    }
                }
            }
        }
    }
}
