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

    public class NBTInt(int val) : NBTNumber<int>(val, "") { }
    public class NBTShort(short val) : NBTNumber<short>(val, "s") { }
    public class NBTLong(long val) : NBTNumber<long>(val, "l") { }
    public class NBTFloat(float val) : NBTNumber<float>(val, "f") { }
    public class NBTDouble(double val) : NBTNumber<double>(val, "d") { }
    public class NBTByte(byte val) : NBTNumber<byte>(val, "b") { }
}
