using Datapack.Net.Data;
using Geode.Errors;
using Geode.IR.Passes;
using Geode.Values;
using System;

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
	}
}
