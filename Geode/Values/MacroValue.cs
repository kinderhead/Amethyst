using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Geode.Values
{
	public class MacroValue(string name, TypeSpecifier type) : Value, IConstantValue
	{
		public readonly string Name = name;
		public override TypeSpecifier Type => type;

		public NBTValue Value => new NBTRawString(GetMacro());

		public string GetMacro() => $"$({Name})";

		public override ScoreValue AsScore(RenderContext ctx) => throw new InvalidOperationException();
		public override bool Equals(object? obj) => obj is MacroValue m && m.Name == Name;
		public override void If(Action<Execute> apply, RenderContext ctx, int tmp = 0) => throw new InvalidOperationException();
		public override FormattedText Render(FormattedText text, RenderContext ctx) => text.Text(GetMacro());
		public override string ToString() => GetMacro();
		public override int GetHashCode() => HashCode.Combine(Name, Type);
	}
}
