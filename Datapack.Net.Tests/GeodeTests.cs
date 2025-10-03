namespace Datapack.Net.Tests
{
	public class GeodeTests
	{
		//private static FunctionContext GetCtx() => new(new(new Options()), new("test:main", new(Amethyst.AST.FunctionModifiers.None, new VoidTypeSpecifier(), [])));

		//[Test]
		//public void TestLifetime()
		//{
		//    var ctx = GetCtx();

		//    var entry = ctx.CurrentBlock;

		//    var x = new Variable("x", "x", PrimitiveTypeSpecifier.Int);
		//    ctx.Add(new StoreInsn(x, new LiteralValue(0)));

		//    var a = ctx.Add(new AddInsn(x, new LiteralValue(4)), "%a");
		//    var b = ctx.Add(new MulInsn(x, new LiteralValue(-5)), "%b");
		//    ValueRef? c = null;
		//    var cond = ctx.Add(new EqInsn(a, new LiteralValue(4)));

		//    var (ifTrue, ifFalse) = ctx.Branch(cond, "if", () =>
		//    {
		//        c = ctx.Add(new AddInsn(a, new LiteralValue(8)), "%c");
		//        ctx.Add(new StoreInsn(x, c));
		//    }, () => ctx.Add(new StoreInsn(x, a)));

		//    ctx.Add(new StoreInsn(x, b));
		//    ctx.Add(new ReturnInsn());

		//    var ifEnd = ctx.CurrentBlock;
		//    ctx.Finish();

		//    new ResolvePass().Apply(ctx);

		//    var inout = new InOutPass();
		//    inout.Apply(ctx);

		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([]), inout.Ins[entry]);
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([a, b]), inout.Ins[ifTrue]);
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([a, b]), inout.Ins[ifFalse]);
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([b]), inout.Ins[ifEnd]);

		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([a, b]), inout.Outs[entry]);
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([b]), inout.Outs[ifTrue]);
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([b]), inout.Outs[ifFalse]);
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([]), inout.Outs[ifEnd]);

		//    var graphPass = new LifetimePass(inout);
		//    graphPass.Apply(ctx);
		//    var graph = graphPass.Graphs[ctx];

		//    if (c is null) throw new InvalidOperationException();

		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([b]), graph.Graph[a].Edges.Select(i => i.Value));
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([a, c]), graph.Graph[b].Edges.Select(i => i.Value));
		//    CollectionAssert.AreEquivalent(new HashSet<ValueRef>([b]), graph.Graph[c].Edges.Select(i => i.Value));

		//    var regs = graph.CalculateDSatur();

		//    Assert.That(regs[a], Is.EqualTo(regs[c]));
		//    Assert.That(regs[a], Is.Not.EqualTo(regs[b]));

		//    var dump = ctx.Dump();
		//    Debug.WriteLine(dump);
		//}
	}
}
