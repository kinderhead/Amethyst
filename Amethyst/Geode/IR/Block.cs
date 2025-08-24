using System.Text;
using Datapack.Net.Function;
using Datapack.Net.Utils;

namespace Amethyst.Geode.IR
{
    public class Block(string name, NamespacedID funcID) : IInstructionArg
    {
        public string Name => name;

        public bool NeedsScoreReg => false;

        public readonly List<Instruction> Instructions = [];
        public readonly List<Block> Previous = [];
        public readonly List<Block> Next = [];

        public readonly MCFunction Function = new(funcID);

		public HashSet<ValueRef> Dependencies { get; } = [];

		public ValueRef Add(Instruction insn)
        {
            Instructions.Add(insn);
            return insn.ReturnValue;
        }

        public void InsertAtBeginning(params IEnumerable<Instruction> insns) => Instructions.InsertRange(0, insns);

        public void Link(Block next)
        {
            Next.Add(next);
            next.Previous.Add(this);
        }

        public string Dump(Func<IInstructionArg, string> valueMap)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"{Name}:");
            foreach (var i in Instructions)
            {
                builder.AppendLine($"    {i.Dump(valueMap)}");
            }

            return builder.ToString();
        }

        public void Render(GeodeBuilder builder, FunctionContext ctx)
        {
            foreach (var i in Instructions)
            {
                i.Render(new(Function, this, builder, ctx));
            }

            builder.Register(Function);
        }
    }
}
