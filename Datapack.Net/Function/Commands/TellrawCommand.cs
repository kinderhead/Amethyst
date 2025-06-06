using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class TellrawCommand(IEntityTarget targets, FormattedText msg, bool macro = false) : Command(msg.Macro || macro)
    {
        public readonly IEntityTarget Targets = targets;
        public readonly FormattedText Message = msg;

        protected override string PreBuild()
        {
            return $"tellraw {Targets.Get()} {Message}";
        }
    }
}
