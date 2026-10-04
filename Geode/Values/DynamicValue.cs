using System.Text;
using Datapack.Net.Function;
using Datapack.Net.NumberProviders;
using Geode.Errors;

namespace Geode.Values
{
    public class DynamicValue(TypeSpecifier type) : Value(type), IDataWritable, IAdvancedMacroValue
    {
        private readonly List<IValue> parts = [];

        public IConstantValue Macroize(Func<IValue, IConstantValue> apply)
        {
            var builder = new StringBuilder();

            foreach (var i in parts)
            {
                builder.Append(apply(i).Value);
            }

            return LiteralValue.Raw(builder.ToString());
        }

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();

        public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException("Currently cannot display dynamic value");
        public override NumberProvider ToCompute(RenderContext ctx) => throw new ComputeError(this);

        public void StoreTo(DataTargetValue val, RenderContext ctx) =>
            ctx.Macroize([this], (args, ctx) =>
            {
                // Macroize returns NBTRawString, so make it a regular string to add quotes
                val.Store(new LiteralValue(args[0].Value.Build(), Type), ctx);
            });

        public DynamicValue Add(IValue val)
        {
            parts.Add(val);
            return this;
        }

        public DynamicValue Add(string str) => Add(LiteralValue.Raw(str));
    }
}