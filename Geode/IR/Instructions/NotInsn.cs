using Datapack.Net.Data;
using Geode.Chains;
using Geode.Types;
using Geode.Values;

namespace Geode.IR.Instructions
{
	public class NotInsn(ExecuteChain val) : Instruction([val])
	{
		public override string Name => "not";
		public override NBTType?[] ArgTypes => [null];
		public override TypeSpecifier ReturnType => PrimitiveType.Bool;

		public override void Render(RenderContext ctx)
		{
			var cond = Arg<ExecuteChain>(0);
			var ret = ReturnValue.Expect<LValue>();

			ret.Store(new LiteralValue(true), ctx);
			cond.Run(ctx.WithFaux(ctx => ret.Store(new LiteralValue(false), ctx)).Single(), ctx);
		}

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap)
		{
			foreach (var i in Arg<ExecuteChain>(0).Dependencies)
			{
				markOverlap(i, ReturnValue);
			}
		}

		protected override IValue? ComputeReturnValue(FunctionContext ctx)
		{
			var chain = Arg<ExecuteChain>(0);

			if (chain.Chain[0] is not IfValueChain valChain || valChain.Values[0].Value is not LiteralValue l)
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
