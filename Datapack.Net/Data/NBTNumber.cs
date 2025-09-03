using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
	public interface INBTNumber
    {
        public object RawValue { get; }
    }


	public abstract class NBTNumber<T>(T val, string postfix) : NBTValue, INBTNumber where T : INumber<T>
    {
        public readonly T Value = val;
        protected readonly string Postfix = postfix;
		public object RawValue => Value;

		public override void Build(StringBuilder sb)
        {
            sb.Append(Value);
            sb.Append(Postfix);
        }

		public override NBTValue Cast(NBTNumberType type)
		{
			return type switch
			{
				NBTNumberType.Boolean => Convert.ToBoolean(RawValue),
				NBTNumberType.Byte => Convert.ToSByte(RawValue),
				NBTNumberType.Short => Convert.ToInt16(RawValue),
				NBTNumberType.Int => Convert.ToInt32(RawValue),
				NBTNumberType.Long => Convert.ToInt64(RawValue),
				NBTNumberType.Float => Convert.ToSingle(RawValue),
				NBTNumberType.Double => Convert.ToDouble(RawValue),
				_ => throw new NotImplementedException(),
			};
		}
	}

	public class NBTInt(int val) : NBTNumber<int>(val, "")
    {
        public override NBTType Type => NBTType.Int;

		public static implicit operator NBTInt(int val) => new(val);
        public static implicit operator int(NBTInt val) => val.Value;
    }

    public class NBTShort(short val) : NBTNumber<short>(val, "s")
    {
		public override NBTType Type => NBTType.Short;
		public static implicit operator NBTShort(short val) => new(val);
        public static implicit operator short(NBTShort val) => val.Value;
    }

    public class NBTLong(long val) : NBTNumber<long>(val, "l")
    {
		public override NBTType Type => NBTType.Long;
		public static implicit operator NBTLong(long val) => new(val);
        public static implicit operator long(NBTLong val) => val.Value;
    }

    public class NBTFloat(float val) : NBTNumber<float>(val, "f")
    {
		public override NBTType Type => NBTType.Float;
		public static implicit operator NBTFloat(float val) => new(val);
        public static implicit operator float(NBTFloat val) => val.Value;
    }

    public class NBTDouble(double val) : NBTNumber<double>(val, "d")
    {
		public override NBTType Type => NBTType.Double;
		public static implicit operator NBTDouble(double val) => new(val);
        public static implicit operator double(NBTDouble val) => val.Value;
    }

    public class NBTByte(sbyte val) : NBTNumber<sbyte>(val, "b")
    {
		public override NBTType Type => NBTType.Byte;
		public static implicit operator NBTByte(sbyte val) => new(val);
        public static implicit operator sbyte(NBTByte val) => val.Value;
    }
}
