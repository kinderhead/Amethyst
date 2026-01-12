using System.Numerics;
using System.Text;

namespace Datapack.Net.Data
{
	public interface INBTNumber
	{
		object RawValue { get; }
	}

	public abstract class NBTNumber<T, TSelf>(T val, string postfix) : NBTValue, INBTNumber, IComparisonOperators<TSelf, TSelf, bool> where T : INumber<T> where TSelf : NBTNumber<T, TSelf>
	{
		public readonly T Value = val;
		protected readonly string Postfix = postfix;
		public object RawValue => Value;

		public override void Build(StringBuilder sb)
		{
			sb.Append(Value);
			sb.Append(Postfix);
		}

		public override NBTValue Cast(NBTNumberType type) => type switch
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

		public override bool Equals(object? obj)
		{
			if (obj is NBTNumber<T, TSelf> n)
			{
				return this == n;
			}

			return false;
		}

		public override int GetHashCode() => Value.GetHashCode() * 1061;

		static bool IEqualityOperators<TSelf, TSelf, bool>.operator ==(TSelf? left, TSelf? right) => left is not null && right is not null && left.Value == right.Value;
		static bool IEqualityOperators<TSelf, TSelf, bool>.operator !=(TSelf? left, TSelf? right) => !(left is not null && right is not null && left.Value == right.Value);
		static bool IComparisonOperators<TSelf, TSelf, bool>.operator <(TSelf left, TSelf right) => left.Value < right.Value;
		static bool IComparisonOperators<TSelf, TSelf, bool>.operator >(TSelf left, TSelf right) => left.Value > right.Value;
		static bool IComparisonOperators<TSelf, TSelf, bool>.operator <=(TSelf left, TSelf right) => left.Value <= right.Value;
		static bool IComparisonOperators<TSelf, TSelf, bool>.operator >=(TSelf left, TSelf right) => left.Value >= right.Value;

		public static bool operator ==(NBTNumber<T, TSelf> left, NBTNumber<T, TSelf> right) => left.Value == right.Value;
		public static bool operator !=(NBTNumber<T, TSelf> left, NBTNumber<T, TSelf> right) => !(left.Value == right.Value);
		public static bool operator <(NBTNumber<T, TSelf> left, NBTNumber<T, TSelf> right) => left.Value < right.Value;
		public static bool operator >(NBTNumber<T, TSelf> left, NBTNumber<T, TSelf> right) => left.Value > right.Value;
		public static bool operator <=(NBTNumber<T, TSelf> left, NBTNumber<T, TSelf> right) => left.Value <= right.Value;
		public static bool operator >=(NBTNumber<T, TSelf> left, NBTNumber<T, TSelf> right) => left.Value >= right.Value;
	}

	public class NBTInt(int val) : NBTNumber<int, NBTInt>(val, "")
	{
		public override NBTType Type => NBTType.Int;

		public static implicit operator NBTInt(int val) => new(val);
		public static implicit operator int(NBTInt val) => val.Value;
	}

	public class NBTShort(short val) : NBTNumber<short, NBTShort>(val, "s")
	{
		public override NBTType Type => NBTType.Short;
		public static implicit operator NBTShort(short val) => new(val);
		public static implicit operator short(NBTShort val) => val.Value;
	}

	public class NBTLong(long val) : NBTNumber<long, NBTLong>(val, "l")
	{
		public override NBTType Type => NBTType.Long;
		public static implicit operator NBTLong(long val) => new(val);
		public static implicit operator long(NBTLong val) => val.Value;
	}

	public class NBTFloat(float val) : NBTNumber<float, NBTFloat>(val, "f")
	{
		public override NBTType Type => NBTType.Float;
		public static implicit operator NBTFloat(float val) => new(val);
		public static implicit operator float(NBTFloat val) => val.Value;
	}

	public class NBTDouble(double val) : NBTNumber<double, NBTDouble>(val, "d")
	{
		public override NBTType Type => NBTType.Double;
		public static implicit operator NBTDouble(double val) => new(val);
		public static implicit operator double(NBTDouble val) => val.Value;
	}

	public class NBTByte(sbyte val) : NBTNumber<sbyte, NBTByte>(val, "b")
	{
		public override NBTType Type => NBTType.Byte;
		public static implicit operator NBTByte(sbyte val) => new(val);
		public static implicit operator sbyte(NBTByte val) => val.Value;
	}
}
