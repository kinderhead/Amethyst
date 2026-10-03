using Geode.IR.Instructions;
using Geode.Values;

namespace Geode.IR.Passes
{
	public class Mem2RegPass : Pass<Mem2RegPass.State>
	{
		public override int MinimumOptimizationLevel => 1;
		protected override bool SkipBlocks => true; // Handle manually
		protected override bool SkipInsns => true;

		protected override void OnFunction(FunctionContext ctx, ref State state)
		{
			state = new();

			var dominanceFrontiers = ctx.CalculateDominanceFrontiers();

			foreach (var variable in ctx.AllVariables.Where(i => i is { ForceStack: false, Type.ShouldStoreInScore: true }))
			{
				HashSet<Block> hasStore = [..ctx.Blocks.Where(i => i.ContainsStoreFor(variable))];
				Stack<Block> stack = new(hasStore.Reverse());
				state.PhiLocations[variable] = [];

				while (stack.Count != 0)
				{
					var block = stack.Pop();
					foreach (var frontier in dominanceFrontiers[block])
					{
						if (state.PhiLocations[variable].Add(frontier))
						{
							if (!hasStore.Contains(frontier))
							{
								stack.Push(frontier);
							}
						}
					}
				}

				foreach (var i in state.PhiLocations[variable])
				{
					i.Prepend(new PhiInsn(variable));
				}
			}

			OnBlock(ctx, ctx.Start, state);
		}

		protected override void OnBlock(FunctionContext ctx, Block block, State state)
		{
			ValueRef decide(Variable variable)
			{
				foreach (var i in state.ValueStack)
				{
					if (i.TryGetValue(variable, out var ret))
					{
						return ret;
					}
				}

				throw new InvalidOperationException("Error picking variable for Mem2Reg. Report the issue and turn off optimizations.");
			}

			bool usesVariable(Variable variable) => state.PhiLocations.ContainsKey(variable);

			foreach (var phi in block.PhiInsns.Where(i => usesVariable(i.Variable)))
			{
				state.ValueStack.Peek()[phi.Variable] = phi.ReturnValue;
			}

			if (Visited(block))
			{
				return;
			}

			MarkVisited(block);

			foreach (var i in block.Instructions)
			{
				switch (i)
				{
					case ILoadInsn { Variable.Value: Variable v1 } load when usesVariable(v1):
					{
						var val = decide(v1);
						load.Remove();
						ctx.ReplaceValue(load.ReturnValue, val);
						break;
					}
					case IStoreInsn { Variable.Value: Variable v2 } store when usesVariable(v2):
						state.ValueStack.Peek()[v2] = store.Value;
						store.Remove();
						break;
					default:
					{
						// Iterate with ValueRefs instead of Variables to keep references correct
						foreach (var variable in i.Dependencies.Where(i => i.Value is Variable v && usesVariable(v)))
						{
							var val = decide((Variable?)variable.Value!);
							i.ReplaceValue(variable, val);
						}

						break;
					}
				}

				if (i is IBranchInsn branch)
				{
					foreach (var dest in branch.Destinations)
					{
						foreach (var phi in dest.PhiInsns.Where(i => usesVariable(i.Variable)))
						{
							var val = decide(phi.Variable);
							phi.Add(block, val);
						}

						state.ValueStack.Push([]);
						OnBlock(ctx, dest, state);
						state.ValueStack.Pop();
					}
				}
			}
		}

		public class State
		{
			public readonly Dictionary<Variable, HashSet<Block>> PhiLocations = []; // I suppose keep this around for the unit tests
			public readonly Stack<Dictionary<Variable, ValueRef>> ValueStack = new([[]]);
		}
	}
}