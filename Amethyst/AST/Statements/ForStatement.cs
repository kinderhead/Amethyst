using Geode;
using Geode.IR;

namespace Amethyst.AST.Statements
{
	public class ForStatement(
		LocationRange loc,
		Statement? initializer,
		Expression condition,
		Expression iterator,
		Statement body) : Statement(loc)
	{
		public readonly Statement Body = body;
		public readonly Expression Condition = condition;
		public readonly Statement? Initializer = initializer;
		public readonly Expression Iterator = iterator;

		public override void Compile(FunctionContext ctx)
		{
			Initializer?.Compile(ctx);
			ctx.Loop(() =>
			{
				var chain = new ExecuteChain();
				Condition.ExecuteChain(chain, ctx);
				return chain;
			}, "for", () =>
			{
				Body.Compile(ctx);
			}, () =>
			{
				Iterator.Execute(ctx, null);
			});
		}
	}
}