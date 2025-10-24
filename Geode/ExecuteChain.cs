using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Geode.IR;
using Geode.Values;

namespace Geode
{
	public class ExecuteChain : IInstructionArg
	{
		public readonly List<ExecuteChainSubcommand> Chain = [];
		public string Name => "chain";
		public HashSet<ValueRef> Dependencies => [.. Chain.SelectMany(i => i.Values)];

		public bool Forks => Chain.Any(i => i.Forks);

		public void Add(ExecuteChainSubcommand cmd) => Chain.Add(cmd);

		public void Run(Command ifTrue, RenderContext ctx)
		{
			var cmd = new Execute();

			ctx.Builder.Macroizer.Run(ctx, GetDependencies(), (args, ctx) =>
			{
				Compute(cmd, () => ifTrue, args, ctx);
			});
		}

		public void RunWithPropagate(Func<NBTCompound, Command> ifTrue, RenderContext ctx)
		{
			var cmd = new Execute();

			ctx.Builder.Macroizer.RunAndPropagateMacros(ctx, GetDependencies(), (args, macros, ctx) =>
			{
				Compute(cmd, () => ifTrue(macros), args, ctx);
			});
		}

		private IEnumerable<IValue> GetDependencies()
		{
			return Chain.Where(i => i.RequireLiteral).SelectMany(i => i.Values.Select(i => i.Expect()));
		}

		private void Compute(Execute cmd, Func<Command> ifTrue, IConstantValue[] args, RenderContext ctx)
		{
			int idex = 0;

			bool? ret = null;

			foreach (var i in Chain)
			{
				if (i.RequireLiteral)
				{
					ret = i.Build(args[idex..(idex + i.Values.Length)], ctx, cmd);
					idex += i.Values.Length;
				}
				else
				{
					ret = i.Build([.. i.Values.Select(i => i.Expect())], ctx, cmd);
				}

				if (ret is not null)
				{
					break;
				}
			}

			if (ret.HasValue)
			{
				if (ret.Value)
				{
					ctx.Add(ifTrue());
				}

				return;
			}

			ctx.Add(cmd.Run(ifTrue()));
		}
	}

	public abstract class ExecuteChainSubcommand(IEnumerable<ValueRef> vals)
	{
		public ValueRef[] Values = [.. vals];

		public virtual bool RequireLiteral => false;
		public virtual bool Forks => false;

		/// <summary>
		/// Process the execute chain subcommand
		/// </summary>
		/// <param name="processedArgs"><see cref="Values"> processed in accordance with <see cref="RequireLiteral"></param>
		/// <param name="ctx"/>Render context</param>
		/// <param name="cmd">Execute command</param>
		/// <returns>True if the result is always true, false if the result is always false, null otherwise</returns>
		public abstract bool? Build(IValue[] processedArgs, RenderContext ctx, Execute cmd);
	}

	public abstract class ExecuteChainConditional(IEnumerable<ValueRef> vals, bool invert) : ExecuteChainSubcommand(vals)
	{
		public bool Invert = invert;

		public override bool? Build(IValue[] processedArgs, RenderContext ctx, Execute cmd) => Build(processedArgs, ctx, Invert ? cmd.Unless : cmd.If);
		protected abstract bool? Build(IValue[] processedArgs, RenderContext ctx, Execute.Conditional cmd);
	}
}
