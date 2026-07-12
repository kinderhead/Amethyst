using Geode;
using Geode.Errors;
using Geode.IR;
using System.Diagnostics;

namespace Amethyst.AST
{
	public class RootNode(LocationRange loc, Compiler ctx) : Node(loc)
	{
		public readonly List<IRootChild> Children = [];
		public readonly Compiler Ctx = ctx;
		public readonly List<FunctionNode> Functions = [];

		public bool BuildSymbols()
		{
			var success = true;

			foreach (var i in Children)
			{
				if (!Ctx.WrapError(i.Location, [DebuggerNonUserCode]() =>
				    {
					    i.Process(Ctx, this);
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
				try
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
				catch (GeodeError e)
				{
					e.Display(Ctx, i.Location);
					success = false;
				}
			}

			return success;
		}
	}

	public interface IRootChild
	{
		LocationRange Location { get; }

		void Process(Compiler ctx, RootNode root);
	}
}