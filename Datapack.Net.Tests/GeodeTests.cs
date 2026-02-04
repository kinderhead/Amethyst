using Amethyst;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.IR.Passes;
using Geode.Types;
using Geode.Values;
using Block = Geode.IR.Block;

namespace Datapack.Net.Tests
{
	public class GeodeTests : Mem2RegPass
	{
		private static FunctionContext GetCtx(TypeSpecifier returnType) => new(new Compiler(new() { Inputs = [], Output = "" }), new("test:main", new(FunctionModifiers.None, returnType, []), LocationRange.None), [], LocationRange.None);

		// Uses the example graph from https://longfangsong.github.io/en/mem2reg-made-simple/. Block 4 may or may not be wrong since the article didn't mention its dominance frontier set.
		private static FunctionContext GetMem2RegCtx(out Variable a, out Block b1, out Block b2, out Block b3, out Block b4, out Block b5, out Block b6, out Block b7, out Block b8)
		{
			var ctx = GetCtx(PrimitiveType.Int);

			Block block(string name)
			{
				var b = new Block(name, ctx.GetNewInternalID(), ctx);
				ctx.Add(b);
				return b;
			}

			a = ctx.RegisterLocal("a", PrimitiveType.Int, LocationRange.None);

			b1 = ctx.Start;
			b1.Add(new StoreInsn(a, new(new LiteralValue(1))));

			b2 = block("b2");
			var a0 = b2.Add(new LoadInsn(a), "a0");
			var t0 = b2.Add(new AddInsn(a0, new LiteralValue(1)), "t0");
			b2.Add(new StoreInsn(a, t0));

			b3 = block("b3");
			var a1 = b3.Add(new LoadInsn(a), "a1");
			var t1 = b3.Add(new AddInsn(a1, a1), "t1");
			b3.Add(new StoreInsn(a, t1));

			b4 = block("b4");
			b5 = block("b5");
			var a2 = b5.Add(new LoadInsn(a), "a2");
			b5.Add(new StoreInsn(a, new LiteralValue(2)));

			b6 = block("b6");
			var a3 = b6.Add(new LoadInsn(a), "a3");
			b6.Add(new StoreInsn(a, new LiteralValue(3)));

			b7 = block("b7");
			var a4 = b7.Add(new LoadInsn(a), "a4");

			b8 = block("b8");
			var a5 = b8.Add(new LoadInsn(a), "a5");
			b8.Add(new ReturnInsn(a5));

			b1.LinkNext(b2);
			b1.LinkNext(b4);
			b4.LinkNext(b5);
			b4.LinkNext(b6);
			b2.LinkNext(b3);
			b5.LinkNext(b7);
			b6.LinkNext(b7);
			b3.LinkNext(b8);
			b3.LinkNext(b2);
			b7.LinkNext(b8);

			ctx.Finish();

			return ctx;
		}

		[Test]
		public void TestDominanceFrontier()
		{
			var ctx = GetMem2RegCtx(out Variable a, out Block b1, out Block b2, out Block b3, out Block b4, out Block b5, out Block b6, out Block b7, out Block b8);

			var doms = ctx.CalculateDominanceFrontiers();

			CollectionAssert.AreEquivalent(new HashSet<Block>([]), doms[b1]);
			CollectionAssert.AreEquivalent(new HashSet<Block>([b2, b8]), doms[b2]);
			CollectionAssert.AreEquivalent(new HashSet<Block>([b2, b8]), doms[b3]);
			CollectionAssert.AreEquivalent(new HashSet<Block>([b8]), doms[b4]);
			CollectionAssert.AreEquivalent(new HashSet<Block>([b7]), doms[b5]);
			CollectionAssert.AreEquivalent(new HashSet<Block>([b7]), doms[b6]);
			CollectionAssert.AreEquivalent(new HashSet<Block>([b8]), doms[b7]);
			CollectionAssert.AreEquivalent(new HashSet<Block>([]), doms[b8]);
		}

		[Test]
		public void TestPhiAllocation()
		{
			var ctx = GetMem2RegCtx(out Variable a, out Block b1, out Block b2, out Block b3, out Block b4, out Block b5, out Block b6, out Block b7, out Block b8);

			State state = new();
			OnFunction(ctx, ref state);

			Assert.That(state, Is.Not.Null);
			CollectionAssert.AreEquivalent(new HashSet<Block>([b2, b7, b8]), state.PhiLocations[a]);
		}
	}
}
