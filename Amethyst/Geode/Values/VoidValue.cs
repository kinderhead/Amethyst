using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public class VoidValue : Value
    {
        public override TypeSpecifier Type => new VoidTypeSpecifier();
        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0) => throw new NotImplementedException(); // Ideally this shouldn't happen, also idk how to make the execute always fail consistently
        public override bool Equals(object? obj) => obj is VoidValue;
        public override int GetHashCode() => 0; // hmm
        public override FormattedText Render(FormattedText text) => text.Text("void");
    }
}
