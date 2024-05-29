using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class RandomCommand(MCRange<int> range, bool value = true, bool macro = false) : Command(macro)
    {
        public readonly bool Value = value;
        public readonly MCRange<int> Range = range;

        protected override string PreBuild()
        {
            return $"random {(Value ? "value" : "roll")} {Range}";
        }
    }
}
