using Geode.Errors;

namespace Geode.IR
{
	public interface IPass
	{
		public int MinimumOptimizationLevel { get; }

		public void Apply(FunctionContext ctx);
	}

	public abstract class Pass<T> : IPass
	{
		public virtual int MinimumOptimizationLevel => 0;

		protected virtual bool SkipBlocks => false;
		protected virtual bool SkipInsns => false;
		protected virtual bool Reversed => false;
		private HashSet<Block> toVisit = [];

		public void Apply(FunctionContext ctx)
		{
			if (!ctx.IsFinished)
			{
				throw new InvalidOperationException($"Function {ctx.Decl.ID} is not finished");
			}

			T? state = default;
			toVisit = [.. ctx.Blocks];
			OnFunction(ctx, ref state!);

			if (!SkipBlocks)
			{
				if (Reversed)
				{
					Walk(ctx, ctx.ExitBlock, ref state);
				}
				else
				{
					Walk(ctx, ctx.Start, ref state);
				}

				while (toVisit.Count != 0)
				{
					ProcessBlock(ctx, toVisit.First(), state);
				}

				// Do this here in case OnBlock modifies other blocks
				foreach (var b in ctx.Blocks)
				{
					b.Cleanse();
				}
			}
		}

		private void Walk(FunctionContext ctx, Block b, ref T state)
		{
			if (!ProcessBlock(ctx, b, state))
			{
				return;
			}

			foreach (var i in Reversed ? b.Previous : b.Next)
			{
				Walk(ctx, i, ref state);
			}
		}

		protected bool ProcessBlock(FunctionContext ctx, Block b, T state)
		{
			if (Visited(b))
			{
				return false;
			}

			MarkVisited(b);

			OnBlock(ctx, b, state);

			if (!SkipInsns)
			{
				foreach (var i in Reversed ? b.Instructions.AsEnumerable().Reverse() : b.Instructions)
				{
					if (!ctx.Compiler.WrapError(i.Location, [System.Diagnostics.DebuggerNonUserCode] () =>
					{
						OnInsn(ctx, b, i, state);
					}))
					{
						throw new EmptyGeodeError();
					}
				}
			}

			return true;
		}

		protected void RevisitBlock(Block b, bool visitPredecessors = false)
		{
			toVisit.Add(b);
			if (visitPredecessors)
			{
				foreach (var i in b.Previous)
				{
					toVisit.Add(i);
				}
			}
		}

		protected void MarkVisited(Block b) => toVisit.Remove(b);
		protected bool Visited(Block b) => !toVisit.Contains(b);

		protected virtual void OnFunction(FunctionContext ctx, ref T state) { }
		protected virtual void OnBlock(FunctionContext ctx, Block block, T state) { }
		protected virtual void OnInsn(FunctionContext ctx, Block block, Instruction insn, T state) { }
	}

	public abstract class Pass : Pass<object>
	{
		protected override void OnFunction(FunctionContext ctx, ref object state) => OnFunction(ctx);
		protected virtual void OnFunction(FunctionContext ctx) { }

		protected override void OnBlock(FunctionContext ctx, Block block, object state) => OnBlock(ctx, block);
		protected virtual void OnBlock(FunctionContext ctx, Block block) { }

		protected override void OnInsn(FunctionContext ctx, Block block, Instruction insn, object state) => OnInsn(ctx, block, insn);
		protected virtual void OnInsn(FunctionContext ctx, Block block, Instruction insn) { }
	}
}
