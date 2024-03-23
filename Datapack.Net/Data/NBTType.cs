using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public abstract class NBTType
    {
        public abstract void Build(StringBuilder sb);

        public virtual string Build()
        {
            StringBuilder sb = new();
            Build(sb);
            return sb.ToString();
        }

        public override string ToString()
        {
            return Build();
        }
    }

    public enum NBTNumberType
    {
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double
    }
}
