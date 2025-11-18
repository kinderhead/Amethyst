using Geode.Errors;

namespace Geode.IR
{
	public abstract class Pass
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

			OnFunction(ctx);

			if (!SkipBlocks)
			{
				toVisit = [.. ctx.Blocks];
				if (Reversed)
				{
					Walk(ctx, ctx.ExitBlock);
				}
				else
				{
					Walk(ctx, ctx.Start);
				}

				while (toVisit.Count != 0)
				{
					ProcessBlock(ctx, toVisit.First());
				}
			}
		}

		private void Walk(FunctionContext ctx, Block b)
		{
			if (!ProcessBlock(ctx, b))
			{
				return;
			}

			foreach (var i in Reversed ? b.Previous : b.Next)
			{
				Walk(ctx, i);
			}
		}

		private bool ProcessBlock(FunctionContext ctx, Block b)
		{
			if (!toVisit.Contains(b))
			{
				return false;
			}

			toVisit.Remove(b);

			OnBlock(ctx, b);

			if (!SkipInsns)
			{
				foreach (var i in Reversed ? b.Instructions.AsEnumerable().Reverse() : b.Instructions)
				{
					if (!ctx.Compiler.WrapError(i.Location, () =>
					{
						OnInsn(ctx, b, i);
					}))
					{
						throw new EmptyGeodeError();
					}
				}
			}

			b.Instructions.RemoveAll(x => x.MarkedForRemoval);

			for (var i = 0; i < b.Instructions.Count; i++)
			{
				if (b.Instructions[i].ToReplaceWith.Length != 0)
				{
					b.Instructions.InsertRange(i + 1, b.Instructions[i].ToReplaceWith);
					b.Instructions.RemoveAt(i);
					i--;
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

		protected virtual void OnFunction(FunctionContext ctx) { }
		protected virtual void OnBlock(FunctionContext ctx, Block block) { }
		protected virtual void OnInsn(FunctionContext ctx, Block block, Instruction insn) { }
	}
}
