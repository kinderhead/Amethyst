namespace Datapack.Net.Data
{
	public class MCRange<T> where T : struct, IEquatable<T>, IConvertible
	{
		public T? Min;
		public T? Max;

		public MCRange(T min, T max)
		{
			Min = min;
			Max = max;
		}

		/// <summary>
		/// Create a range from one number.
		/// If gte is null, than only a single value is returned.
		/// If gte is true, then the range will accept numbers greater than or equal to val.
		/// If gte is false, then the range will accept numbers less than or equal to val.
		/// </summary>
		/// <param name="val">Value</param>
		/// <param name="gte">Greater than or equal to</param>
		public MCRange(T val, bool? gte = null)
		{
			if (gte == null)
			{
				Min = val;
				Max = val;
			}
			else if ((bool)gte)
			{
				Min = val;
				Max = null;
			}
			else
			{
				Max = val;
			}
		}

		public override string ToString()
		{
			if (Min != null && Max != null)
			{
				if (Min.Equals(Max))
				{
					return $"{Min}";
				}
				else
				{
					return $"{Min}..{Max}";
				}
			}
			else if (Max == null)
			{
				return $"{Min}..";
			}
			else
			{
				return $"..{Max}";
			}
		}

		public static implicit operator MCRange<T>(T val) => new(val);
		public static implicit operator MCRange<T>((T, T) val) => new(val.Item1, val.Item2);
	}
}
