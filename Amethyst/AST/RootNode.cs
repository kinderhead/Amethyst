using Amethyst.Geode.IR;

namespace Amethyst.AST
{
	public class RootNode(LocationRange loc, Compiler ctx) : Node(loc)
	{
		public readonly Compiler Ctx = ctx;
		public readonly List<FunctionNode> Functions = [];
		public readonly List<IRootChild> Children = [];

		public bool BuildSymbols()
		{
			var success = true;

			foreach (var i in Children.Concat(Functions))
			{
				if (!Ctx.WrapError(i.Location, () =>
				{
					i.Process(Ctx);
				}))
				{
					success = false;
				}
			}

			return success;
		}

		public bool CompileFunctions(out List<FunctionContext> funcs)
		{
			var success = true;
			funcs = [];

			foreach (var i in Functions)
			{
				if (!i.Compile(Ctx, out var ctx) || ctx is null)
				{
					success = false;
				}
				else
				{
					funcs.Add(ctx);
				}
			}

			return success;
		}
	}

	public interface IRootChild
	{
		LocationRange Location { get; }

		void Process(Compiler ctx);
	}
}
