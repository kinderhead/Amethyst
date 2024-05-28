using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public static class Extensions
    {
        public static FormattedText Score(this FormattedText text, ScoreRef val)
        {
            return text.Score(val.Target, val.Score);
        }

        public static Execute Store(this Execute cmd, ScoreRef score, bool result = true) => cmd.Store(score.Target, score.Score, result);
    }
}
