namespace Datapack.Net.Data
{
	public class Negatable<T>(T val, bool negative = false)
	{
		public readonly bool Negative = negative;
		public readonly T Value = val;

		public override string ToString()
		{
			if (Negative)
			{
				return $"!{Value}";
			}

			return $"{Value}";
		}
	}
}