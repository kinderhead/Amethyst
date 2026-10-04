using Amethyst.IR.Types;
using Datapack.Net.Function;
using Datapack.Net.NumberProviders;
using Geode;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR
{
    public class NullValue() : StoreableValue(new ReferenceType(new VoidType(), false))
    {
        public override ScoreValue AsScore(RenderContext ctx) => ctx.Builder.Constant(0);
        public override IValue AsStoreable() => Type.DefaultValue;
        public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Text("null");
        public override NumberProvider ToCompute(RenderContext ctx) => new ConstantIntProvider(0, ctx.Builder.RandomID);

        public override string ToString() => "null";
    }
}