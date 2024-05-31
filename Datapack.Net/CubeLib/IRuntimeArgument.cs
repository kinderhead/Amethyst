using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public interface IRuntimeArgument
    {
        public ScoreRef GetAsArg();

        public static virtual IRuntimeArgument Create(ScoreRef arg) => throw new NotImplementedException();
    }
}
