using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.CubeLib
{
    public interface IRuntimeArgument
    {
        /// <summary>
        /// Get the <see cref="ScoreRef"/> representation of the object.
        /// </summary>
        /// <returns></returns>
        public ScoreRef GetAsArg();

        /// <summary>
        /// Creates the object from a <see cref="ScoreRef"/>.
        /// One class in the inheritance chain must define this.
        /// </summary>
        /// <param name="arg">Score</param>
        /// <returns>The created object</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static virtual IRuntimeArgument Create(ScoreRef arg) => throw new NotImplementedException();
    }
}
