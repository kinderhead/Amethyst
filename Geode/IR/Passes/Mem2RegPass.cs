
using Geode.Values;

namespace Geode.IR.Passes
{
    public class Mem2RegPass : Pass<Mem2RegPass.State>
    {
		public override int MinimumOptimizationLevel => 1;
		protected override bool SkipInsns => true; // Handle manually

		protected override void OnFunction(FunctionContext ctx, ref State state)
        {
            state = new();

            var dominanceFrontiers = ctx.CalculateDominanceFrontiers();

            foreach (var variable in ctx.AllVariables.Where(i => !i.HasReference && i.Type.ShouldStoreInScore))
            {
                HashSet<Block> hasStore = [..ctx.Blocks.Where(i => i.ContainsStoreFor(variable))];
                Stack<Block> stack = new(hasStore.Reverse());
                state.PhiLocations[variable] = [];

                while (stack.Count != 0)
                {
                    var block = stack.Pop();
                    foreach (var frontier in dominanceFrontiers[block])
                    {
                        state.PhiLocations[variable].Add(frontier);
                        if (!hasStore.Contains(frontier))
                        {
                            stack.Push(frontier);
                        }
                    }
                }
            }
        }

		protected override void OnBlock(FunctionContext ctx, Block block, State state)
        {
			(Block Block, IValue Value) decide(Variable variable)
			{
				foreach (var i in state.ValueStack.Reverse())
				{
					if (i.TryGetValue(variable, out var ret))
					{
						return ret;
					}
				}

				throw new InvalidOperationException("Error picking variable for Mem2Reg. Report the issue and turn off optimizations.");
			}

			foreach (var (variable, blocks) in state.PhiLocations)
            {
                var val = decide(variable);
                
            }

            MarkVisited(block);
            if (Visited(block))
            {
                return;
            }


        }

        public class State
        {
            public readonly Dictionary<Variable, HashSet<Block>> PhiLocations = [];
            public readonly Stack<Dictionary<Variable, (Block Block, IValue Value)>> ValueStack = new([[]]);
        }
    }
}
