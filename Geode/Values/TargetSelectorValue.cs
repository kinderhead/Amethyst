using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.NumberProviders;
using Geode.Errors;
using Geode.Types;
using Geode.Util;

namespace Geode.Values
{
    public class TargetSelectorValue(TargetType type, MultiDictionary<string, IValue> args) : Value(new TargetSelectorType()), IDataWritable, IAdvancedMacroValue
    {
        public readonly MultiDictionary<string, IValue> Arguments = args;
        public readonly TargetType TargetType = type;

        public IConstantValue Macroize(Func<IValue, IConstantValue> apply)
        {
            var newArgs = new MultiDictionary<string, IValue>();

            foreach (var (k, v) in Arguments)
            {
                if (v is DataTargetValue nbt && v.Type == PrimitiveType.String)
                {
                    // Ignore macro string warnings
                    newArgs.Add(k, apply(new RawDataTargetValue(nbt.Target.GetTarget(), PrimitiveType.Compound)));
                }
                else
                    newArgs.Add(k, apply(v));
            }

            return new LiteralValue(new NBTRawString(new TargetSelectorValue(TargetType, newArgs).ToString()), Type);
        }

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidTypeError(Type.ToString(), "int");

        public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException("Entity printing is not implemented yet");
        public override NumberProvider ToCompute(RenderContext ctx) => throw new ComputeError(this);

        public void StoreTo(DataTargetValue val, RenderContext ctx)
        {
            if (Arguments["name"].Concat(Arguments["!name"]).Any(i => i is not IConstantValue)) throw new TargetSelectorMacroArgumentError("name");

            if (Arguments["nbt"].Concat(Arguments["!nbt"]).Any(i => i is not IConstantValue)) throw new TargetSelectorMacroArgumentError("nbt");

            ctx.Macroize([this], (args, ctx) =>
            {
                // Macroize returns NBTRawString, so make it a regular string to add quotes
                val.Store(new LiteralValue(args[0].Value.Build(), Type), ctx);
            });
        }

        public bool IsSingle()
        {
            int? limit = null;

            foreach (var (k, v) in Arguments)
            {
                if (k == "limit")
                {
                    if (v is LiteralValue l && l.Is<NBTInt>(out var nbt)) limit = nbt.Value;

                    break;
                }
            }

            switch (TargetType)
            {
                case TargetType.e or TargetType.a when limit == 1:
                case TargetType.s when limit is null:
                case TargetType.p or TargetType.r when limit is null or 1:
                    return true;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            var target = new MultiDictionary<string, string>();

            foreach (var (k, v) in Arguments)
            {
                var arg = k;
                var negated = false;

                if (k.StartsWith('!'))
                {
                    arg = k[1..];
                    negated = true;
                }

                if (v is LiteralValue literal && literal.Is<NBTString>(out var str) && str.Value.Contains('!')) throw new TargetSelectorNegatedLiteralError(str.Value);

                string val;

                // Remove quotes if constant
                if ((arg is "type" || v.Type is RangeType) && v is IConstantValue c && c.Value is NBTString str2)
                    val = str2.Value;
                else
                    val = v.ToString() ?? "";

                switch (arg)
                {
                    case "sort" when v is IConstantValue { Value: NBTString { Value: not "nearest" and not "furthest" and not "random" and not "arbituary" } str3 }:
                        throw new TargetSelectorInvalidSortError(str3.Value);
                    case "gamemode" when v is IConstantValue { Value: NBTString { Value: not "survival" and not "creative" and not "spectator" and not "adventure" } str4 }:
                        throw new TargetSelectorInvalidSortError(str4.Value);
                }

                if (negated)
                    target.Add(arg, '!' + val);
                else
                    target.Add(arg, val);
            }

            return $"{TargetSelector.GetTypeName(TargetType)}{(target.Count > 0 ? $"[{TargetSelector.CompileDict(target)}]" : "")}";
        }
    }
}