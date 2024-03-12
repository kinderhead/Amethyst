using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public abstract class Command(bool macro)
    {
        public readonly bool Macro = macro;

        public string Build()
        {
            if (Macro) return "$" + PreBuild();
            return PreBuild();
        }

        protected abstract string PreBuild();
    }
}
