using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public class MacroValue(string name, TypeSpecifier type) : Value
    {
        public readonly string Name = name;
        public override TypeSpecifier Type => type;
        public override bool IsLiteral => true;

        public string GetMacro(bool includeGuards = true) => includeGuards ? $"{Type.MacroGuardStart}$({Name}){Type.MacroGuardEnd}" : $"$({Name})";

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
        public override bool Equals(object? obj) => this is MacroValue m && m.Name == Name;
        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0) => throw new InvalidOperationException();
        public override FormattedText Render(FormattedText text) => text.Text(GetMacro(false)); // No guards in text
        public override string ToString() => GetMacro();
        public override int GetHashCode() => HashCode.Combine(Name, Type);
    }
}
