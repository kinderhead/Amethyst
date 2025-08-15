using System.Text;

namespace Amethyst.Geode.IR
{
    public class Block(string name) : IInstructionArg
    {
        public string Name => name;

        public bool NeedsScoreReg => false;

        public readonly List<Instruction> Instructions = [];
        public readonly List<Block> Previous = [];
        public readonly List<Block> Next = [];

        public ValueRef Add(Instruction insn)
        {
            Instructions.Add(insn);
            return insn.ReturnValue;
        }

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
    }
}
