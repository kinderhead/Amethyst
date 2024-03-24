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

        public static implicit operator NBTType(string val) => new NBTString(val);
        public static implicit operator NBTType(int val) => new NBTInt(val);
        public static implicit operator NBTType(byte val) => new NBTByte(val);
        public static implicit operator NBTType(short val) => new NBTShort(val);
        public static implicit operator NBTType(long val) => new NBTLong(val);
        public static implicit operator NBTType(float val) => new NBTFloat(val);
        public static implicit operator NBTType(double val) => new NBTDouble(val);
        public static implicit operator NBTType(bool val) => new NBTBool(val);

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
