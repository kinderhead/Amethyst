using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class KillCommand(IEntityTarget targets, bool macro = false) : Command(macro)
    {
        public readonly IEntityTarget Targets = targets;

        protected override string PreBuild() => $"kill {Targets.Get()}";
    }
}
