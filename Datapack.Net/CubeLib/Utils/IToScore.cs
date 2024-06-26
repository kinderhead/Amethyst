using System;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib.Utils
{
    public abstract class ToScoreUtil
    {
        public abstract ScoreRef ToScore();

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static ScoreRefComparison operator ==(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Op = Comparison.Equal };
        public static ScoreRefComparison operator ==(int a, ToScoreUtil b) => new() { Left = a, RightScore = b.ToScore(), Op = Comparison.Equal };
        public static ScoreRefComparison operator ==(ToScoreUtil a, int b) => new() { LeftScore = a.ToScore(), Right = b, Op = Comparison.Equal };

        public static ScoreRefMatches operator ==(ToScoreUtil a, MCRange<int> b) => new() { Score = a.ToScore(), Range = b };
        public static ScoreRefMatches operator ==(MCRange<int> a, ToScoreUtil b) => new() { Score = b.ToScore(), Range = a };
        public static ScoreRefMatches operator !=(MCRange<int> a, ToScoreUtil b) => new() { Score = b.ToScore(), Range = a, If = false };
        public static ScoreRefMatches operator !=(ToScoreUtil a, MCRange<int> b) => new() { Score = a.ToScore(), Range = b, If = false };

        public static ScoreRefComparison operator !=(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Op = Comparison.Equal, If = false };
        public static ScoreRefComparison operator !=(int a, ToScoreUtil b) => new() { Left = a, RightScore = b.ToScore(), Op = Comparison.Equal, If = false };
        public static ScoreRefComparison operator !=(ToScoreUtil a, int b) => new() { LeftScore = a.ToScore(), Right = b, Op = Comparison.Equal, If = false };

        public static ScoreRefComparison operator >(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Op = Comparison.GreaterThan };
        public static ScoreRefComparison operator >(int a, ToScoreUtil b) => new() { Left = a, RightScore = b.ToScore(), Op = Comparison.GreaterThan };
        public static ScoreRefComparison operator >(ToScoreUtil a, int b) => new() { LeftScore = a.ToScore(), Right = b, Op = Comparison.GreaterThan };

        public static ScoreRefComparison operator <(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Op = Comparison.LessThan };
        public static ScoreRefComparison operator <(int a, ToScoreUtil b) => new() { Left = a, RightScore = b.ToScore(), Op = Comparison.LessThan };
        public static ScoreRefComparison operator <(ToScoreUtil a, int b) => new() { LeftScore = a.ToScore(), Right = b, Op = Comparison.LessThan };

        public static ScoreRefComparison operator >=(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Op = Comparison.GreaterThanOrEqual };
        public static ScoreRefComparison operator >=(int a, ToScoreUtil b) => new() { Left = a, RightScore = b.ToScore(), Op = Comparison.GreaterThanOrEqual };
        public static ScoreRefComparison operator >=(ToScoreUtil a, int b) => new() { LeftScore = a.ToScore(), Right = b, Op = Comparison.GreaterThanOrEqual };

        public static ScoreRefComparison operator <=(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Op = Comparison.LessThanOrEqual };
        public static ScoreRefComparison operator <=(int a, ToScoreUtil b) => new() { Left = a, RightScore = b.ToScore(), Op = Comparison.LessThanOrEqual };
        public static ScoreRefComparison operator <=(ToScoreUtil a, int b) => new() { LeftScore = a.ToScore(), Right = b, Op = Comparison.LessThanOrEqual };

        public static ScoreRefOperation operator +(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Operation = ScoreOperation.Add };
        public static ScoreRefOperation operator +(ScoreRefOperation a, ToScoreUtil b) => new() { LeftBranch = a, RightScore = b.ToScore(), Operation = ScoreOperation.Add };
        public static ScoreRefOperation operator +(ToScoreUtil a, ScoreRefOperation b) => new() { LeftScore = a.ToScore(), RightBranch = b, Operation = ScoreOperation.Add };

        public static ScoreRefOperation operator -(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Operation = ScoreOperation.Sub };
        public static ScoreRefOperation operator -(ScoreRefOperation a, ToScoreUtil b) => new() { LeftBranch = a, RightScore = b.ToScore(), Operation = ScoreOperation.Sub };
        public static ScoreRefOperation operator -(ToScoreUtil a, ScoreRefOperation b) => new() { LeftScore = a.ToScore(), RightBranch = b, Operation = ScoreOperation.Sub };

        public static ScoreRefOperation operator *(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Operation = ScoreOperation.Mul };
        public static ScoreRefOperation operator *(ScoreRefOperation a, ToScoreUtil b) => new() { LeftBranch = a, RightScore = b.ToScore(), Operation = ScoreOperation.Mul };
        public static ScoreRefOperation operator *(ToScoreUtil a, ScoreRefOperation b) => new() { LeftScore = a.ToScore(), RightBranch = b, Operation = ScoreOperation.Mul };

        public static ScoreRefOperation operator /(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Operation = ScoreOperation.Div };
        public static ScoreRefOperation operator /(ScoreRefOperation a, ToScoreUtil b) => new() { LeftBranch = a, RightScore = b.ToScore(), Operation = ScoreOperation.Div };
        public static ScoreRefOperation operator /(ToScoreUtil a, ScoreRefOperation b) => new() { LeftScore = a.ToScore(), RightBranch = b, Operation = ScoreOperation.Div };

        public static ScoreRefOperation operator %(ToScoreUtil a, ToScoreUtil b) => new() { LeftScore = a.ToScore(), RightScore = b.ToScore(), Operation = ScoreOperation.Mod };
        public static ScoreRefOperation operator %(ScoreRefOperation a, ToScoreUtil b) => new() { LeftBranch = a, RightScore = b.ToScore(), Operation = ScoreOperation.Mod };
        public static ScoreRefOperation operator %(ToScoreUtil a, ScoreRefOperation b) => new() { LeftScore = a.ToScore(), RightBranch = b, Operation = ScoreOperation.Mod };
    }
}
