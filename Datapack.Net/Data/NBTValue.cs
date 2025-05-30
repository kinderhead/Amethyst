using Datapack.Net.CubeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public abstract class NBTValue : IPointerable
    {
        public abstract NBTType Type { get; }
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

        public NBTNumberType? NumberType
        {
            get
            {
                if (this is NBTInt) return NBTNumberType.Int;
                if (this is NBTLong) return NBTNumberType.Long;
                if (this is NBTShort) return NBTNumberType.Short;
                if (this is NBTFloat) return NBTNumberType.Float;
                if (this is NBTDouble) return NBTNumberType.Double;
                if (this is NBTByte) return NBTNumberType.Byte;
                return null;
            }
        }

        public static NBTNumberType? IsNumberType<T>() where T : NBTValue
        {
            if (typeof(T) == typeof(NBTInt)) return NBTNumberType.Int;
            if (typeof(T) == typeof(NBTLong)) return NBTNumberType.Long;
            if (typeof(T) == typeof(NBTShort)) return NBTNumberType.Short;
            if (typeof(T) == typeof(NBTFloat)) return NBTNumberType.Float;
            if (typeof(T) == typeof(NBTDouble)) return NBTNumberType.Double;
            if (typeof(T) == typeof(NBTByte)) return NBTNumberType.Byte;
            return null;
        }

        public static implicit operator NBTValue(string val) => new NBTString(val);
        public static implicit operator NBTValue(int val) => new NBTInt(val);
        public static implicit operator NBTValue(byte val) => new NBTByte(val);
        public static implicit operator NBTValue(short val) => new NBTShort(val);
        public static implicit operator NBTValue(long val) => new NBTLong(val);
        public static implicit operator NBTValue(float val) => new NBTFloat(val);
        public static implicit operator NBTValue(double val) => new NBTDouble(val);
        public static implicit operator NBTValue(bool val) => new NBTBool(val);

        public static NBTValue? ToNBT(object obj)
        {
            if (obj is NBTValue nbt) return nbt;

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
            if (typeof(T).IsAssignableTo(typeof(NBTValue))) return true;

            return RawNBTTypes.Contains(typeof(T));
        }

        public static readonly Type[] RawNBTTypes = [typeof(string), typeof(int), typeof(byte), typeof(short), typeof(long), typeof(float), typeof(double), typeof(bool), typeof(int[]), typeof(byte[]), typeof(long[])];
    }

    public enum NBTType
    {
		Byte,
        Boolean,
		Short,
		Int,
		Long,
		Float,
		Double,
        String,
        List,
        Compound,
        ByteArray,
        IntArray,
        LongArray
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
