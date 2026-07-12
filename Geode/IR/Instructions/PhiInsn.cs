using Geode.IR.Passes;
using Geode.Values;
using System.Text;

namespace Geode.IR.Instructions
{
	public class PhiInsn(Variable variable) : DynamicInstruction, IPhiLike
	{
		private readonly Dictionary<Block, ValueRef> values = [];

		public readonly Variable Variable = variable;

		private Block? processed;
		public override string Name => "phi";
		public override TypeSpecifier ReturnType => Variable.Type;
		public override bool ArgumentsAliveAtInsn => false;
		public IReadOnlyDictionary<Block, ValueRef> Values => values;

		public override IEnumerable<ValueRef> Dependencies => Values.Values;

		public void Process(Block block)
		{
			processed = block;

			foreach (var (from, val) in values)
			{
				block.Phi.Map(from, val, ReturnValue);
			}
		}

		public override void Render(RenderContext ctx) { }
		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;

		public void Add(Block from, ValueRef val) => values[from] = val;

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink,
			Action<ValueRef, ValueRef> markOverlap)
		{
			foreach (var (_, val) in values)
			{
				tryLink(ReturnValue, val);
			}
		}

		public override void ReplaceValue(ValueRef val, ValueRef with)
		{
			foreach (var (k, v) in values)
			{
				if (v == val)
				{
					values[k] = with;
				}
			}
		}

		public override void Remove()
		{
			base.Remove();

			if (processed is not null)
			{
				foreach (var (from, val) in values)
				{
					processed.Phi.Unmap(from, val);
				}
			}
		}

		public override string Dump()
		{
			var builder = new StringBuilder();

			builder.Append($"{ReturnValue} = {Name} ");

			foreach (var (block, val) in Values)
			{
				builder.Append($"[{block}: {val}], ");
			}

			if (Values.Count > 0)
			{
				builder.Length -= 2;
			}

			return builder.ToString();
		}
	}
}