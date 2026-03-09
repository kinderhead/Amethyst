using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Types;

namespace Geode.Values
{
	public class VoidValue : Value
	{
		public override TypeSpecifier Type => new VoidType();
		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
		public override bool Equals(object? obj) => obj is VoidValue;
		public override int GetHashCode() => 0; // hmm
		public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Text("void");
	}
}
