using System.Text;

namespace Datapack.Net.Data
{
    public abstract class NBTValue
    {
        public static readonly Type[] RawNBTTypes =
        [
            typeof(string), typeof(int), typeof(byte), typeof(short), typeof(long), typeof(float), typeof(double),
            typeof(bool), typeof(int[]), typeof(byte[]), typeof(long[])
        ];

        public abstract NBTType Type { get; }

        public NBTNumberType? NumberType
        {
            get
            {
                return this switch
                {
                    NBTBool => NBTNumberType.Boolean,
                    NBTInt => NBTNumberType.Int,
                    NBTLong => NBTNumberType.Long,
                    NBTShort => NBTNumberType.Short,
                    NBTFloat => NBTNumberType.Float,
                    NBTDouble => NBTNumberType.Double,
                    NBTByte => NBTNumberType.Byte,
                    _ => null
                };
            }
        }

        public abstract void Build(StringBuilder sb);

        public virtual string Build()
        {
            StringBuilder sb = new();
            Build(sb);
            return sb.ToString();
        }

        public override string ToString() => Build();

        public abstract NBTValue Cast(NBTNumberType type);

        public NBTInt CastInt() => (NBTInt)Cast(NBTNumberType.Int);
        public NBTFloat CastFloat() => (NBTFloat)Cast(NBTNumberType.Float);

        public static NBTNumberType? IsNumberType<T>() where T : NBTValue
        {
            if (typeof(T) == typeof(NBTBool)) return NBTNumberType.Boolean;
            if (typeof(T) == typeof(NBTInt)) return NBTNumberType.Int;
            if (typeof(T) == typeof(NBTLong)) return NBTNumberType.Long;
            if (typeof(T) == typeof(NBTShort)) return NBTNumberType.Short;
            if (typeof(T) == typeof(NBTFloat)) return NBTNumberType.Float;
            if (typeof(T) == typeof(NBTDouble)) return NBTNumberType.Double;
            if (typeof(T) == typeof(NBTByte)) return NBTNumberType.Byte;
            return null;
        }

        public static bool IsNumberType(NBTType type) => type switch
        {
            NBTType.Boolean or NBTType.Byte or NBTType.Short or NBTType.Int or NBTType.Float or NBTType.Double => true,
            _ => false
        };

        public static bool IsOperableType(NBTType type) => type switch
        {
            NBTType.Byte or NBTType.Short or NBTType.Int => true,
            _ => false
        };

        public static implicit operator NBTValue(string val) => new NBTString(val);
        public static implicit operator NBTValue(int val) => new NBTInt(val);
        public static implicit operator NBTValue(sbyte val) => new NBTByte(val);
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
                sbyte b => new NBTByte(b),
                short s => new NBTShort(s),
                long l => new NBTLong(l),
                float f => new NBTFloat(f),
                double d => new NBTDouble(d),
                bool tf => new NBTBool(tf),
                _ => null
            };
        }

        public static bool IsNBTType<T>() => typeof(T).IsAssignableTo(typeof(NBTValue)) || RawNBTTypes.Contains(typeof(T));
    }

    public enum NBTType
    {
        Boolean,
        Byte,
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
        Boolean,
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double
    }
}