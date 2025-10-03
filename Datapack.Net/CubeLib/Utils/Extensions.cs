using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib.Utils
{
	public static class Extensions
	{
		public static FormattedText Score(this FormattedText text, ScoreRef val) => text.Score(val.Target, val.Score);

		public static Execute Store(this Execute cmd, ScoreRef score, bool result = true) => cmd.Store(score.Target, score.Score, result);
		public static Execute Score(this Execute.Conditional cmd, ScoreRef score, MCRange<int> range) => cmd.Score(score.Target, score.Score, range);

		public static bool SameType(this Type type, Type? other)
		{
			if (other == null)
			{
				return false;
			}
			else if (type.IsGenericType != other.IsGenericType)
			{
				return false;
			}
			else if (type.IsGenericType)
			{
				return type.GetGenericTypeDefinition() == other.GetGenericTypeDefinition();
			}
			else
			{
				return type == other;
			}
		}
	}
}
