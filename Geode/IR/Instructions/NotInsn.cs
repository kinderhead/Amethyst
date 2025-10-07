using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class NotInsn(ValueRef val) : Instruction([val])
	{
		public override string Name => "not";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Bool;

		public override void Render(RenderContext ctx)
		{
			var raw = Arg<ValueRef>(0).Expect();

			if (raw is ConditionalValue c)
			{
				c.Flip = !c.Flip;
				ReturnValue.SetValue(c);
				return;
			}

			var ret = ReturnValue.Expect<ScoreValue>();
			ScoreValue val;

			if (raw is ScoreValue s)
			{
				val = s;
			}
			else
			{
				val = ctx.Builder.Temp(0);
				val.Store(raw, ctx);
			}

			ctx.Add(new Execute().Unless.Score(val.Target, val.Score, 0).Run(ctx.WithFaux(ctx => ret.Store(new LiteralValue(0), ctx)).Single()));
			ctx.Add(new Execute().If.Score(val.Target, val.Score, 0).Run(ctx.WithFaux(ctx => ret.Store(new LiteralValue(1), ctx)).Single()));
		}

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap) => markOverlap(Arg<ValueRef>(0), ReturnValue);

		protected override Value? ComputeReturnValue(FunctionContext ctx)
		{
			var val = Arg<ValueRef>(0).Value;

			if (val is ConditionalValue c)
			{
				return c;
			}

			if (val is not LiteralValue l)
			{
				return null;
			}

			if (l.Value is NBTByte b)
			{
				return new LiteralValue(new NBTBool(b.Value == 0));
			}
			else if (l.Value is NBTInt i)
			{
				return new LiteralValue(new NBTBool(i.Value == 0));
			}
			else if (l.Value is NBTLong n)
			{
				return new LiteralValue(new NBTBool(n.Value == 0));
			}
			else if (l.Value is NBTShort s)
			{
				return new LiteralValue(new NBTBool(s.Value == 0));
			}
			else if (l.Value is NBTFloat f)
			{
				return new LiteralValue(new NBTBool(f.Value == 0));
			}
			else if (l.Value is NBTDouble d)
			{
				return new LiteralValue(new NBTBool(d.Value == 0));
			}
			else if (l.Value is NBTString str)
			{
				return new LiteralValue(new NBTBool(string.IsNullOrEmpty(str.Value)));
			}
			else if (l.Value is NBTList list)
			{
				return new LiteralValue(new NBTBool(list.Count == 0));
			}
			else if (l.Value is NBTCompound comp)
			{
				return new LiteralValue(new NBTBool(comp.Count == 0));
			}
			else
			{
				throw new NotImplementedException($"Cannot negate literal {l.Value}");
			}
		}
	}
}
