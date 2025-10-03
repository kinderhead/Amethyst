using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public abstract class PropertyValue(TypeSpecifier type) : LValue
	{
		public override TypeSpecifier Type => type;

		public override ScoreValue AsScore(RenderContext ctx) => throw new NotImplementedException();

		public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0)
		{
			var val = ctx.Builder.Temp(tmp);
			Get(val, ctx);
			return val.If(cmd, ctx, tmp++);
		}

		public abstract void Get(LValue dest, RenderContext ctx);

		public override FormattedText Render(FormattedText text, RenderContext ctx)
		{
			var tmp = GeodeBuilder.TempStorage(Type);
			Get(tmp, ctx);
			return tmp.Render(text, ctx);
		}
	}
}
