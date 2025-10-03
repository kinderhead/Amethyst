using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
	public class ScoreRefOperation : ToScoreUtil
	{
		public ScoreRef? LeftScore;
		public ScoreRef? RightScore;
		public ScoreRefOperation? LeftBranch;
		public ScoreRefOperation? RightBranch;
		public ScoreOperation Operation;

		public void Process(ScoreRef dest, int tmp = 0) => Resolve(dest, tmp);

		public override ScoreRef ToScore() => Resolve(Project.ActiveProject.Local(), 0);

		private ScoreRef Resolve(ScoreRef dest, int tmp)
		{
			if (LeftScore is not null)
			{
				dest.Set(LeftScore);
			}
			else if (LeftBranch is not null)
			{
				_ = LeftBranch.Resolve(dest, tmp);
			}
			else
			{
				throw new ArgumentException("Malformed ScoreRefOperation");
			}

			if (RightScore is not null)
			{
				dest.Op(RightScore, Operation);
			}
			else if (RightBranch is not null)
			{
				dest.Op(RightBranch.Resolve(Project.ActiveProject.Temp(tmp, "math"), tmp + 1), Operation);
			}
			else
			{
				throw new ArgumentException("Malformed ScoreRefOperation");
			}

			return dest;
		}

		public static ScoreRefOperation operator +(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Add };
		public static ScoreRefOperation operator +(int a, ScoreRefOperation b) => Project.ActiveProject.Constant(a) + b;
		public static ScoreRefOperation operator +(ScoreRefOperation a, int b) => a + Project.ActiveProject.Constant(b);

		public static ScoreRefOperation operator -(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Sub };
		public static ScoreRefOperation operator -(int a, ScoreRefOperation b) => Project.ActiveProject.Constant(a) - b;
		public static ScoreRefOperation operator -(ScoreRefOperation a, int b) => a - Project.ActiveProject.Constant(b);

		public static ScoreRefOperation operator *(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Mul };
		public static ScoreRefOperation operator *(int a, ScoreRefOperation b) => Project.ActiveProject.Constant(a) * b;
		public static ScoreRefOperation operator *(ScoreRefOperation a, int b) => a * Project.ActiveProject.Constant(b);

		public static ScoreRefOperation operator /(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Div };
		public static ScoreRefOperation operator /(int a, ScoreRefOperation b) => Project.ActiveProject.Constant(a) / b;
		public static ScoreRefOperation operator /(ScoreRefOperation a, int b) => a / Project.ActiveProject.Constant(b);

		public static ScoreRefOperation operator %(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Mod };
		public static ScoreRefOperation operator %(int a, ScoreRefOperation b) => Project.ActiveProject.Constant(a) % b;
		public static ScoreRefOperation operator %(ScoreRefOperation a, int b) => a % Project.ActiveProject.Constant(b);

		public static implicit operator ScoreRef(ScoreRefOperation a)
		{
			var tmp = Project.ActiveProject.Temp(0, "math");
			a.Process(tmp);
			return tmp;
		}
	}
}
