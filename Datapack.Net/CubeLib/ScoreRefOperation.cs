using System;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
    public class ScoreRefOperation
    {
        public ScoreRef? LeftScore;
        public ScoreRef? RightScore;
        public ScoreRefOperation? LeftBranch;
        public ScoreRefOperation? RightBranch;
        public int LeftConst;
        public int RightConst;
        public ScoreOperation Operation;

        public void Process(ScoreRef dest, int tmp = 0) => Resolve(dest, tmp);

        private ScoreRef Resolve(ScoreRef dest, int tmp)
        {
            if (LeftScore is not null) dest.Set(LeftScore);
            else if (LeftBranch is not null) LeftBranch.Resolve(dest, tmp);
            else dest.Set(LeftConst);

            if (RightScore is not null) dest.Op(RightScore, Operation);
            else if (RightBranch is not null) dest.Op(RightBranch.Resolve(Project.ActiveProject.Temp(tmp, "math"), tmp + 1), Operation);
            else dest.Op(RightConst, Operation);

            return dest;
        }

        public static ScoreRefOperation operator +(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Add };
        public static ScoreRefOperation operator +(int a, ScoreRefOperation b) => new() { LeftConst = a, RightBranch = b, Operation = ScoreOperation.Add };
        public static ScoreRefOperation operator +(ScoreRefOperation a, int b) => new() { LeftBranch = a, RightConst = b, Operation = ScoreOperation.Add };

        public static ScoreRefOperation operator -(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Sub };
        public static ScoreRefOperation operator -(int a, ScoreRefOperation b) => new() { LeftConst = a, RightBranch = b, Operation = ScoreOperation.Sub };
        public static ScoreRefOperation operator -(ScoreRefOperation a, int b) => new() { LeftBranch = a, RightConst = b, Operation = ScoreOperation.Sub };

        public static ScoreRefOperation operator *(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Mul };
        public static ScoreRefOperation operator *(int a, ScoreRefOperation b) => new() { LeftConst = a, RightBranch = b, Operation = ScoreOperation.Mul };
        public static ScoreRefOperation operator *(ScoreRefOperation a, int b) => new() { LeftBranch = a, RightConst = b, Operation = ScoreOperation.Mul };

        public static ScoreRefOperation operator /(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Div };
        public static ScoreRefOperation operator /(int a, ScoreRefOperation b) => new() { LeftConst = a, RightBranch = b, Operation = ScoreOperation.Div };
        public static ScoreRefOperation operator /(ScoreRefOperation a, int b) => new() { LeftBranch = a, RightConst = b, Operation = ScoreOperation.Div };

        public static ScoreRefOperation operator %(ScoreRefOperation a, ScoreRefOperation b) => new() { LeftBranch = a, RightBranch = b, Operation = ScoreOperation.Mod };
        public static ScoreRefOperation operator %(int a, ScoreRefOperation b) => new() { LeftConst = a, RightBranch = b, Operation = ScoreOperation.Mod };
        public static ScoreRefOperation operator %(ScoreRefOperation a, int b) => new() { LeftBranch = a, RightConst = b, Operation = ScoreOperation.Mod };
    }
}
