using Amethyst.Geode.IR;
using Amethyst.Geode.Values;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode
{
    public record class RenderContext(MCFunction MCFunction, Block Block, GeodeBuilder Builder, FunctionContext Func)
    {
        public virtual void Add(params IEnumerable<Command> cmds)
        {
            if (Func.IsMacroFunction) MCFunction.Add(cmds.Select(i =>
            {
                i.Macro = true;
                return i;
            }));
            else MCFunction.Add(cmds);
        }

        public void StoreCompound(StorageValue dest, Dictionary<string, ValueRef> dict, bool setEmpty = true)
        {
            var ret = StoreCompoundOrReturnConstant(dest, dict, setEmpty);
            if (ret is LiteralValue l) dest.Store(l, this);
        }

        public Value StoreCompoundOrReturnConstant(StorageValue dest, Dictionary<string, ValueRef> dict, bool setEmpty = true)
        {
            var nbt = new Datapack.Net.Data.NBTCompound();
            var runtime = new Dictionary<string, Value>();

            foreach (var (key, val) in dict.Select(i => (i.Key, i.Value.Expect())))
            {
                if (val.IsLiteral) nbt[key] = ((LiteralValue)val).Value;
                else runtime[key] = val;
            }

            if (nbt.Count == dict.Count) return new LiteralValue(nbt);

            if (setEmpty || !(nbt.Count == 0)) dest.Store(new LiteralValue(nbt), this);

            foreach (var (key, value) in runtime)
            {
                dest.Property(key, value.Type).Store(value, this);
            }

            return dest;
        }

        public Command CallFunction(MCFunction func)
        {
            if (Func.IsMacroFunction) return new FunctionCommand(func, [.. Func.Decl.FuncType.Parameters.Where(i => i.Modifiers.HasFlag(AST.ParameterModifiers.Macro)).Select(i => new KeyValuePair<string, Datapack.Net.Data.NBTValue>(i.Name, $"$({i.Name})"))]);
            else return new FunctionCommand(func);
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
