using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib.Utils
{
    public static class Extensions
    {
        public static FormattedText Score(this FormattedText text, ScoreRef val)
        {
            return text.Score(val.Target, val.Score);
        }

        public static Execute Store(this Execute cmd, ScoreRef score, bool result = true) => cmd.Store(score.Target, score.Score, result);
        public static Execute Score(this Execute.Conditional cmd, ScoreRef score, MCRange<int> range) => cmd.Score(score.Target, score.Score, range);
    }
}
