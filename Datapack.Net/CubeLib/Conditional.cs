using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public abstract class Conditional
    {
        /// <summary>
        /// Execute operation, false for unless.
        /// </summary>
        public bool If = true;

        public abstract Execute Process(Execute cmd, int tmp = 0);

        public static Conditional operator !(Conditional op)
        {
            var inverse = (Conditional)op.MemberwiseClone();
            inverse.If = !inverse.If;
            return inverse;
        }
    }
}
