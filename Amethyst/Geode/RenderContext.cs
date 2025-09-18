using Amethyst.Errors;
using Amethyst.Geode.IR;
using Amethyst.Geode.Values;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

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

        public void StoreCompound(DataTargetValue dest, Dictionary<string, ValueRef> dict, bool setEmpty = true)
        {
            var ret = StoreCompoundOrReturnConstant(dest, dict, setEmpty);
            if (ret is LiteralValue l) dest.Store(l, this);
        }

        public Value StoreCompoundOrReturnConstant(DataTargetValue dest, Dictionary<string, ValueRef> dict, bool setEmpty = true)
        {
            var nbt = new Datapack.Net.Data.NBTCompound();
            var runtime = new Dictionary<string, Value>();
            var forceUseStorage = false;

            foreach (var (key, val) in dict.Select(i => (i.Key, i.Value.Expect())))
            {
                if (val is VoidValue) forceUseStorage = true;
                else if (val is ILiteralValue l) nbt[key] = l.Value;
                else runtime[key] = val;
            }

            if (!forceUseStorage || nbt.Count == dict.Count) return new LiteralValue(nbt);

            if (setEmpty || nbt.Count != 0) dest.Store(new LiteralValue(nbt), this);

            foreach (var (key, value) in runtime)
            {
                dest.Property(key, value.Type).Store(value, this);
            }

            return dest;
        }

        public Command CallSubFunction(MCFunction func)
        {
            if (Func.IsMacroFunction) return new FunctionCommand(func, [.. Func.Decl.FuncType.Parameters.Where(i => i.Modifiers.HasFlag(AST.ParameterModifiers.Macro)).Select(i => new KeyValuePair<string, Datapack.Net.Data.NBTValue>(i.Name, $"\"$({i.Name})\""))]);
            else return new FunctionCommand(func);
        }

        public void Call(NamespacedID id, bool applyGuard, params ValueRef[] args)
        {
            if (Func.GetGlobal(id) is not FunctionValue func) throw new UndefinedSymbolError(id.ToString());
            func.Call(this, args, applyGuard);
        }

        public void Call(NamespacedID id, params ValueRef[] args) => Call(id, true, args);

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
