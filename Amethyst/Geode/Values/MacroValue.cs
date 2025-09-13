using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public class MacroValue(string name, TypeSpecifier type) : Value
    {
        public readonly string Name = name;
        public override TypeSpecifier Type => type;
        public override bool IsLiteral => true;

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
        public override bool Equals(object? obj) => this is MacroValue m && m.Name == Name;
        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0) => throw new InvalidOperationException();
        public override FormattedText Render(FormattedText text) => text.Text(ToString());
        public override string ToString() => $"$({Name})";
        public override int GetHashCode() => HashCode.Combine(Name, Type);
    }
}
