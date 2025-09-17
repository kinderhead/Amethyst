using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
    public class MacroValue(string name, TypeSpecifier type) : Value, ILiteralValue
    {
        public readonly string Name = name;
        public override TypeSpecifier Type => type;

        public NBTValue Value => new NBTRawString(GetMacro());

        public string GetMacro() => $"$({Name})";

        public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
        public override bool Equals(object? obj) => this is MacroValue m && m.Name == Name;
        public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0) => throw new InvalidOperationException();
        public override FormattedText Render(FormattedText text) => text.Text(GetMacro());
        public override string ToString() => GetMacro();
        public override int GetHashCode() => HashCode.Combine(Name, Type);

        //public MacroValue Property(string prop, TypeSpecifier type) => new($"0{Name}_{prop}", type); // Macros can't have the "." operator. Be careful to include enough 0s
    }
}
