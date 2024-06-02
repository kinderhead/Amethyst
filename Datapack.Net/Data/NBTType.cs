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

        public static NBTType? ToNBT(object obj)
        {
            if (obj is NBTType nbt) return nbt;

            return obj switch
            {
                string str => new NBTString(str),
                int i => new NBTInt(i),
                byte b => new NBTByte(b),
                short s => new NBTShort(s),
                long l => new NBTLong(l),
                float f => new NBTFloat(f),
                double d => new NBTDouble(d),
                bool tf => new NBTBool(tf),
                _ => null
            };
        }

        public static bool IsNBTType<T>()
        {
            if (typeof(T).IsAssignableTo(typeof(NBTType))) return true;

            return RawNBTTypes.Contains(typeof(T));
        }

        public static readonly Type[] RawNBTTypes = [typeof(string), typeof(int), typeof(byte), typeof(short), typeof(long), typeof(float), typeof(double), typeof(bool)];
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
