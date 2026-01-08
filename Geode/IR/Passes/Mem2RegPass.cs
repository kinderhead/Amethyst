
using Geode.IR.Instructions;
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

            bool usesVariable(Variable variable) => state.PhiLocations.ContainsKey(variable);

			foreach (var (variable, _) in state.PhiLocations.Where(i => i.Value.Contains(block)))
            {
                var val = decide(variable);
                //block.Prepend()
            }
            
            if (Visited(block))
            {
                return;
            }

            MarkVisited(block);

            foreach (var i in block.Instructions)
            {
                if (i is ILoadInsn load && load.Variable.Value is Variable v && usesVariable(v))
                {
                    load.Remove();
                }
            }
        }

        public class State
        {
            public readonly Dictionary<Variable, HashSet<Block>> PhiLocations = [];
            public readonly Stack<Dictionary<Variable, (Block Block, IValue Value)>> ValueStack = new([[]]);
        }
    }
}
