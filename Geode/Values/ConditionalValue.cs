using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.Types;

namespace Geode.Values
{
	public class ConditionalValue(Func<Execute, bool, Execute> apply, bool flip = false) : Value
	{
		public readonly Func<Execute, bool, Execute> Apply = apply;
		public bool Flip = flip;
		public override TypeSpecifier Type => PrimitiveType.Bool;

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException("Cannot implicitly convert a conditional to a score");
		public override bool Equals(object? obj) => obj is ConditionalValue c && c.Apply == Apply; // I don't like this but oh well
		public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0) => Apply(cmd, Flip);
		public override int GetHashCode() => Apply.GetHashCode();

		public override FormattedText Render(FormattedText text, RenderContext ctx) => throw new NotImplementedException("Cannot print conditionals at the moment. Assign to a variable for now");
	}
}
