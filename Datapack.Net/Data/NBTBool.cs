using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class NBTBool(bool val) : NBTType
    {
        public readonly bool Value = val;

        public override void Build(StringBuilder sb)
        {
            sb.Append(Value ? "true" : "false");
        }
    }
}
