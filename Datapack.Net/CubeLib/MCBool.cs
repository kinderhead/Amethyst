using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public class MCBool(ScoreRef var)
    {
        public readonly ScoreRef Var = var;

        public MCBool(ScoreRef var, bool val) : this(var)
        {
            Set(val);
        }

        public void Set(bool val) => Var.Set(val ? 1 : 0);

        public static implicit operator ScoreRefComparison(MCBool a) => a.Var == 1;
    }
}
