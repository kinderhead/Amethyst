using Amethyst.Geode.Types;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Amethyst.Geode.Values
{
	public abstract class PropertyValue(TypeSpecifier type) : LValue
	{
		public override TypeSpecifier Type => type;

		public override ScoreValue AsScore(RenderContext ctx)
		{
			throw new NotImplementedException();
		}

		public override Execute If(Execute cmd, RenderContext ctx, int tmp = 0)
		{
			var val = ctx.Builder.Temp(tmp);
			Get(val, ctx);
			return val.If(cmd, ctx, tmp++);
		}

		public abstract void Get(LValue dest, RenderContext ctx);

		public override FormattedText Render(FormattedText text, RenderContext ctx)
		{
			var tmp = ctx.Builder.TempStorage(Type);
			Get(tmp, ctx);
			return tmp.Render(text, ctx);
		}
	}

	public class ReferenceValue(ValueRef ptr) : PropertyValue(((ReferenceTypeSpecifier)ptr.Type).Inner)
	{
		public readonly ValueRef Pointer = ptr;

		public override void Get(LValue dest, RenderContext ctx)
		{
			if (dest is DataTargetValue nbt)
			{
				ctx.Call("amethyst:core/ref/set-ref", ReferenceTypeSpecifier.From(nbt), Pointer);
			}
			else throw new NotImplementedException("Storing references into scores is not real yet");
		}

		public override void ListAdd(Value val, RenderContext ctx)
		{
			throw new NotImplementedException();
		}

		public override void Store(Value val, RenderContext ctx)
		{
			if (val is ReferenceValue r) ctx.Call("amethyst:core/ref/set-ref", Pointer, r.Pointer);
			else if (val is DataTargetValue data) ctx.Call("amethyst:core/ref/set-ref", Pointer, ReferenceTypeSpecifier.From(data));
			else
			{
				if (Pointer.Value is ILiteralValue ptr && ptr.Value is NBTString target) new RawDataTargetValue(target.Value, Type).Store(val, ctx);

				// Stupid minecraft doesn't have a good way to escape strings automatically (so far)
				else if (val is LiteralValue l && l.Value is NBTString str) ctx.Call("amethyst:core/ref/set", Pointer, new LiteralValue($"\"{NBTString.Escape(str.Value)}\""));
				else ctx.Call("amethyst:core/ref/set", Pointer, val);
			}
		}

		public override string ToString() => $"&{Pointer}";

		public override bool Equals(object? obj) => obj is ReferenceValue r && r.Pointer.Equals(Pointer);
		public override int GetHashCode() => Pointer.GetHashCode() * 7919;
	}
}
