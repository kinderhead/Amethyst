using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Geode.Values
{
	public class MacroValue(string name, TypeSpecifier type) : Value(type), IConstantValue
	{
		public readonly string Name = name;

		public NBTValue Value => new NBTRawString(GetMacro());

		public string GetMacro() => $"$({Name})";

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
		public override bool Equals(object? obj) => obj is MacroValue m && m.Name == Name;
		public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Text(GetMacro());
		public override string ToString() => GetMacro();
		public override int GetHashCode() => HashCode.Combine(Name, Type);
	}
}
