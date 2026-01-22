using Datapack.Net.Data;
using Geode.Errors;
using Geode.IR.Passes;
using Geode.Values;
using System;
using System.Text;
using static Datapack.Net.Function.Commands.Execute.Conditional.Subcommand;

namespace Geode.IR.Instructions
{
	public class PhiInsn(Variable variable) : DynamicInstruction, IPhiLike
	{
		public override string Name => "phi";
		public override TypeSpecifier ReturnType => Variable.Type;
		public override bool ArgumentsAliveAtInsn => false;

        public readonly Variable Variable = variable;

        private readonly Dictionary<Block, ValueRef> values = [];
        public IReadOnlyDictionary<Block, ValueRef> Values => values;

		public override IEnumerable<ValueRef> Dependencies => Values.Values;

		public override void Render(RenderContext ctx) { }
		protected override IValue? ComputeReturnValue(FunctionContext ctx) => null;

		private Block? processed = null;

        public void Add(Block from, ValueRef val)
        {
            values[from] = val;
        }

		public override void ConfigureLifetime(Func<ValueRef, ValueRef, bool> tryLink, Action<ValueRef, ValueRef> markOverlap)
        {
            foreach (var (_, val) in values)
            {
                tryLink(ReturnValue, val);
            }
        }

        public void Process(Block block)
        {
			processed = block;

            foreach (var (from, val) in values)
            {
                block.Phi.Map(from, val, ReturnValue);
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

		public override string Dump(Func<IInstructionArg, string> valueMap)
        {
            var builder = new StringBuilder();

            builder.Append($"{valueMap(ReturnValue)} = {Name} ");

            foreach (var (block, val) in Values)
            {
                builder.Append($"[{valueMap(block)}: {valueMap(val)}], ");
            } 

            if (Values.Count > 0)
            {
                builder.Length -= 2;
            }

            return builder.ToString();
        }
	}
}
