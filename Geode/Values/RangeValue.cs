using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.NumberProviders;
using Geode.Errors;

namespace Geode.Values
{
    public class RangeValue(ValueRef min, ValueRef max, TypeSpecifier type) : Value(type), IDataWritable, IAdvancedMacroValue
    {
        public readonly ValueRef Max = max;
        public readonly ValueRef Min = min;

        public IConstantValue Macroize(Func<IValue, IConstantValue> apply) => new LiteralValue(new NBTRawString($"{apply(Min.Expect()).Value}..{apply(Max.Expect()).Value}"), Type);

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidTypeError(Type.ToString(), "int");
        public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException();
        public override NumberProvider ToCompute(RenderContext ctx) => throw new ComputeError(this);

        public void StoreTo(DataTargetValue val, RenderContext ctx) =>
            ctx.Macroize([this], (args, ctx) =>
            {
                // Macroize returns NBTRawString, so make it a regular string to add quotes
                val.Store(new LiteralValue(args[0].Value.Build(), Type), ctx);
            });

        public override string ToString() => $"{Max}..{Min}";
    }
}