using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Datapack.Net.Function.Commands.Execute;

namespace Datapack.Net.CubeLib
{
    public class ScoreRefComparison
    {
        public ScoreRef? LeftScore;
        public ScoreRef? RightScore;
        public int Left;
        public int Right;
        public Comparison Op;

        /// <summary>
        /// Execute operation, false for unless.
        /// </summary>
        public bool If = true;

        public Execute Process(Execute cmd)
        {
            Conditional branch = If ? cmd.If : cmd.Unless;

            ScoreRef a = LeftScore ?? Project.ActiveProject.Temp(0, Left, "cmp");
            ScoreRef b = RightScore ?? Project.ActiveProject.Temp(0, Right, "cmp");

            branch.Score(a.Target, a.Score, Op, b.Target, b.Score);

            return cmd;
        }
    }
}
