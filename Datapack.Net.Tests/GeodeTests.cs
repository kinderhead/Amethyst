using Amethyst;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Block = Geode.IR.Block;

namespace Datapack.Net.Tests
{
	public class GeodeTests
	{
		private static FunctionContext GetCtx() => new(new Compiler(new() { Inputs = [], Output = "" }), new("test:main", new(FunctionModifiers.None, new VoidType(), [])), [], LocationRange.None);

		[Test]
		public void TestDominanceFrontier()
		{
			var ctx = GetCtx();

			Block block(string name)
			{
				var b = new Block(name, ctx.GetNewInternalID(), ctx);
				ctx.Add(b);
				return b;
			}

			var b1 = ctx.Start;
			var b2 = block("b2");
			var b3 = block("b3");
			var b4 = block("b4");
			var b5 = block("b5");
			var b6 = block("b6");
			var b7 = block("b7");
			var b8 = block("b8");
			b8.Add(new ReturnInsn());

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
	}
}
