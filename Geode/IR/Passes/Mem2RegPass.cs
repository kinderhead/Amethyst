
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

                foreach (var i in state.PhiLocations[variable])
                {
                    i.Prepend(new PhiInsn(variable));
                }
            }

            OnBlock(ctx, ctx.Start, state);
        }

		protected override void OnBlock(FunctionContext ctx, Block block, State state)
        {
			(Block Block, ValueRef Value) decide(Variable variable)
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

			foreach (var i in block.PhiInsns)
            {
                var val = decide(i.Variable);
                i.Add(val.Block, val.Value);
                state.ValueStack.Peek()[i.Variable] = (val.Block, i.ReturnValue);
            }
            
            if (Visited(block))
            {
                return;
            }

            MarkVisited(block);

            foreach (var i in block.Instructions)
            {
                if (i is ILoadInsn load && load.Variable.Value is Variable v1 && usesVariable(v1))
                {
                    var (_, val) = decide(v1);
                    load.Remove();
                    ctx.ReplaceValue(load.ReturnValue, val);
                }
                else if (i is IStoreInsn store && store.Variable.Value is Variable v2 && usesVariable(v2))
                {
                    state.ValueStack.Peek()[v2] = (block, store.Value);
                    store.Remove();
                }
                else if (i is IBranchInsn branch)
                {
                    state.ValueStack.Push([]);
                    OnBlock(ctx, branch.TrueBlock, state);
                    state.ValueStack.Pop();

                    state.ValueStack.Push([]);
                    OnBlock(ctx, branch.FalseBlock, state);
                    state.ValueStack.Pop();
                }
                else if (i is IJumpInsn jump)
                {
                    OnBlock(ctx, jump.DestBlock, state);
                }
				else
				{
					// Iterate with ValueRefs instead of Variables to keep references correct
					foreach (var variable in i.Dependencies.Where(i => i.Value is Variable v && usesVariable(v)))
					{
						var (_, val) = decide((Variable?)variable.Value!);
						i.ReplaceValue(variable, val);
					}
				}
            }
        }

        public class State
        {
            public readonly Dictionary<Variable, HashSet<Block>> PhiLocations = []; // I suppose keep this around for the unit tests
            public readonly Stack<Dictionary<Variable, (Block Block, ValueRef Value)>> ValueStack = new([[]]);
        }
    }
}
