using Amethyst.Geode.Types;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public class LiteralValue(NBTValue val, TypeSpecifier? type = null) : Value, IConstantValue
	{
		public NBTValue Value { get; } = val;
		public override TypeSpecifier Type => type ?? new PrimitiveTypeSpecifier(Value.Type);

		public override ScoreValue AsScore(RenderContext ctx) => Value is NBTInt n ? ctx.Builder.Constant(n) : throw new InvalidOperationException($"\"{Value}\" is not an integer");
		public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0) => throw new NotImplementedException(); // Ideally this shouldn't happen
		public override bool Equals(object? obj) => obj is LiteralValue l && l.Value == Value;
		public override string ToString() => Value.ToString();
		public override int GetHashCode() => Value.GetHashCode();
		public override FormattedText Render(FormattedText text, RenderContext ctx) => Value is NBTString str ? text.Text(str.Value) : text.Text(Value.ToString());
	}
}
