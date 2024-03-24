using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public abstract class NBTNumber<T>(T val, string postfix) : NBTType
    {
        public readonly T Value = val;
        protected readonly string Postfix = postfix;

        public override void Build(StringBuilder sb)
        {
            sb.Append(Value);
            sb.Append(Postfix);
        }
    }

    public class NBTInt(int val) : NBTNumber<int>(val, "")
    {
        public static implicit operator NBTInt(int val) => new(val);
        public static implicit operator int(NBTInt val) => val.Value;
    }

    public class NBTShort(short val) : NBTNumber<short>(val, "s")
    {
        public static implicit operator NBTShort(short val) => new(val);
        public static implicit operator short(NBTShort val) => val.Value;
    }

    public class NBTLong(long val) : NBTNumber<long>(val, "l")
    {
        public static implicit operator NBTLong(long val) => new(val);
        public static implicit operator long(NBTLong val) => val.Value;
    }

    public class NBTFloat(float val) : NBTNumber<float>(val, "f")
    {
        public static implicit operator NBTFloat(float val) => new(val);
        public static implicit operator float(NBTFloat val) => val.Value;
    }

    public class NBTDouble(double val) : NBTNumber<double>(val, "d")
    {
        public static implicit operator NBTDouble(double val) => new(val);
        public static implicit operator double(NBTDouble val) => val.Value;
    }

    public class NBTByte(byte val) : NBTNumber<byte>(val, "b")
    {
        public static implicit operator NBTByte(byte val) => new(val);
        public static implicit operator byte(NBTByte val) => val.Value;
    }
}
