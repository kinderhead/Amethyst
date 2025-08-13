using System.Text;

namespace Amethyst.Geode.IR
{
    public class Block(string name)
    {
        public readonly string Name = name;
        public readonly List<Instruction> Instructions = [];
        public readonly List<Block> Previous = [];
        public readonly List<Block> Next = [];

        public ValueRef Add(Instruction insn)
        {
            Instructions.Add(insn);
            return insn.ReturnValue;
        }

        public string Dump(Func<ValueRef, string> valueMap)
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
