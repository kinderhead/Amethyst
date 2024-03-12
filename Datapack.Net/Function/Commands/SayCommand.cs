using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class SayCommand(string message, bool macro = false) : Command(macro)
    {
        public readonly string Message = message;

        protected override string PreBuild()
        {
            return $"say {Message}";
        }
    }
}
