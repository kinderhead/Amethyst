using Datapack.Net.CubeLib.Utils;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
	public class ScoreRef(Score score, IEntityTarget target, bool @readonly = false, bool global = false) : ToScoreUtil, IRuntimeArgument
	{
		public readonly Score Score = score;
		public readonly IEntityTarget Target = target.RequireOne();
		public readonly bool Readonly = @readonly;
		public readonly bool Global = global;

		public void Set(int val)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			Project.ActiveProject.AddCommand(new Scoreboard.Players.Set(Target, Score, val));
		}
		public void Set(ScoreRef score)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			Project.ActiveProject.AddCommand(new Scoreboard.Players.Operation(Target, Score, ScoreOperation.Assign, score.Target, score.Score));
		}
		public void Set(ScoreRefOperation op) => op.Process(this);
		public void Add(int val)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			Project.ActiveProject.AddCommand(new Scoreboard.Players.Add(Target, Score, val));
		}
		public void Add(ScoreRef val) => Op(val, ScoreOperation.Add);
		public void Sub(int val)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			Project.ActiveProject.AddCommand(new Scoreboard.Players.Remove(Target, Score, val));
		}
		public void Sub(ScoreRef val) => Op(val, ScoreOperation.Sub);
		public void Mul(int val) => Op(val, ScoreOperation.Mul);
		public void Mul(ScoreRef val) => Op(val, ScoreOperation.Mul);
		public void Div(int val) => Op(val, ScoreOperation.Div);
		public void Div(ScoreRef val) => Op(val, ScoreOperation.Div);
		public void Mod(int val) => Op(val, ScoreOperation.Mod);
		public void Mod(ScoreRef val) => Op(val, ScoreOperation.Mod);

		public Command Get() => new Scoreboard.Players.Get(Target, Score);
		public Command SetCmd(int val)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			return new Scoreboard.Players.Set(Target, Score, val);
		}

		public void DynSet(string macro)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			Project.ActiveProject.AddCommand(new RawCommand($"scoreboard players set {Target.Get()} {Score} $({macro})", true));
		}

		public Execute Store(bool macro = false)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			return new Execute(macro).Store(this);
		}

		public void Op(int val, ScoreOperation op)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			Op(Project.ActiveProject.Constant(val), op);
		}

		public void Op(ScoreRef val, ScoreOperation op)
		{
			if (Readonly)
			{
				throw new ArgumentException("This score is readonly");
			}

			Project.ActiveProject.AddCommand(new Scoreboard.Players.Operation(Target, Score, op, val.Target, val.Score));
		}

		public ScoreRefComparison Exists() => this >= -2147483648;

		public static implicit operator ScoreRef(int a) => Project.ActiveProject.Constant(a);

		public ScoreRef GetAsArg() => this;

		public ScoreRef AsReadonly() => new(Score, Target, true, Global);

		public static IRuntimeArgument Create(ScoreRef arg) => new ScoreRef(arg.Score, arg.Target, global: arg.Global);

		public override ScoreRef ToScore() => this;
	}
}
