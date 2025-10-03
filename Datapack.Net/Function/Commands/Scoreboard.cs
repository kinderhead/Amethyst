namespace Datapack.Net.Function.Commands
{
	public static class Scoreboard
	{
		public static class Objectives
		{
			public class Add(Score score, bool macro = false) : Command(macro)
			{
				public readonly Score Score = score;

				protected override string PreBuild() => $"scoreboard objectives add {Score} {Score.Criteria} {Score.DisplayName}";
			}

			public class Remove(Score score, bool macro = false) : Command(macro)
			{
				public readonly Score Score = score;

				protected override string PreBuild() => $"scoreboard objectives remove {Score}";
			}
		}

		public static class Players
		{
			public class Get(IEntityTarget target, Score score, bool macro = false) : Command(macro)
			{
				public readonly IEntityTarget Target = target.RequireOne();
				public readonly Score Score = score;

				protected override string PreBuild() => $"scoreboard players get {Target.Get()} {Score}";
			}

			public class Set : Command
			{
				public readonly IEntityTarget Target;
				public readonly Score Score;
				public readonly string Value;

				public Set(IEntityTarget target, Score score, int value, bool macro = false) : base(macro)
				{
					Target = target;
					Score = score;
					Value = value.ToString();
				}

				public Set(IEntityTarget target, Score score, string value) : base(true)
				{
					Target = target;
					Score = score;
					Value = value;
				}

				protected override string PreBuild() => $"scoreboard players set {Target.Get()} {Score} {Value}";
			}

			public class Add(IEntityTarget target, Score score, int value, bool macro = false) : Command(macro)
			{
				public readonly IEntityTarget Target = target;
				public readonly Score Score = score;
				public readonly int Value = value;

				protected override string PreBuild() => $"scoreboard players add {Target.Get()} {Score} {Value}";
			}

			public class Remove(IEntityTarget target, Score score, int value, bool macro = false) : Command(macro)
			{
				public readonly IEntityTarget Target = target;
				public readonly Score Score = score;
				public readonly int Value = value;

				protected override string PreBuild() => $"scoreboard players remove {Target.Get()} {Score} {Value}";
			}

			public class Reset(IEntityTarget target, Score? score = null, bool macro = false) : Command(macro)
			{
				public readonly IEntityTarget Target = target;
				public readonly Score? Score = score;

				protected override string PreBuild() => $"scoreboard players reset {Target.Get()} {(Score == null ? "" : Score)}";
			}

			public class Operation(IEntityTarget target, Score targetScore, ScoreOperation op, IEntityTarget source, Score sourceScore, bool macro = false) : Command(macro)
			{
				public readonly IEntityTarget Target = target;
				public readonly Score TargetScore = targetScore;
				public readonly ScoreOperation Op = op;
				public readonly IEntityTarget Source = source;
				public readonly Score SourceScore = sourceScore;

				protected override string PreBuild() => $"scoreboard players operation {Target.Get()} {TargetScore} {FromOperation(Op)} {Source.Get()} {SourceScore}";
			}

			public static string FromOperation(ScoreOperation op) => op switch
			{
				ScoreOperation.Assign => "=",
				ScoreOperation.Add => "+=",
				ScoreOperation.Sub => "-=",
				ScoreOperation.Mul => "*=",
				ScoreOperation.Div => "/=",
				ScoreOperation.Mod => "%=",
				ScoreOperation.Swap => "><",
				ScoreOperation.Min => "<",
				ScoreOperation.Max => ">",
				_ => throw new ArgumentException("Invalid operation"),
			};
		}
	}

	public enum ScoreOperation
	{
		/// <summary>
		/// =
		/// </summary>
		Assign,
		/// <summary>
		/// +=
		/// </summary>
		Add,
		/// <summary>
		/// -=
		/// </summary>
		Sub,
		/// <summary>
		/// *=
		/// </summary>
		Mul,
		/// <summary>
		/// /=
		/// </summary>
		Div,
		/// <summary>
		/// %=
		/// </summary>
		Mod,
		/// <summary>
		/// &gt;&lt;
		/// </summary>
		Swap,
		/// <summary>
		/// &lt;
		/// </summary>
		Min,
		/// <summary>
		/// &gt;
		/// </summary>
		Max
	}
}
