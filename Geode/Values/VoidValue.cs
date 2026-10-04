using Datapack.Net.Function;
using Datapack.Net.NumberProviders;
using Geode.Types;

namespace Geode.Values
{
    public class VoidValue() : Value(new VoidType())
    {
        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
        public override bool Equals(object? obj) => obj is VoidValue;
        public override int GetHashCode() => 0; // hmm
        public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Text("void");
        public override NumberProvider ToCompute(RenderContext ctx) => new ConstantIntProvider(0, ctx.Builder.RandomID);
    }
}