
using Geode.Values;

namespace Geode.IR.Passes
{
    public class Mem2RegPass : Pass<Mem2RegPass.State>
    {
		public override int MinimumOptimizationLevel => 1;

		protected override void OnFunction(FunctionContext ctx, ref State? state)
        {
            state = [];

            var dominanceFrontiers = ctx.CalculateDominanceFrontiers();

            foreach (var variable in ctx.AllVariables.Where(i => !i.HasReference))
            {
                HashSet<Block> hasStore = [..ctx.Blocks.Where(i => i.ContainsStoreFor(variable))];
                Stack<Block> stack = new(hasStore.Reverse());
                state[variable] = [];

                while (stack.Count != 0)
                {
                    var block = stack.Pop();
                    foreach (var frontier in dominanceFrontiers[block])
                    {
                        state[variable].Add(frontier);
                        if (!hasStore.Contains(frontier))
                        {
                            stack.Push(frontier);
                        }
                    }
                }
            }
        }

        public class State : Dictionary<Variable, HashSet<Block>> { }
    }
}
