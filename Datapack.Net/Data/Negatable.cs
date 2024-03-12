using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class Negatable<T>(T val, bool negative = false)
    {
        public readonly T Value = val;
        public readonly bool Negative = negative;

        public override string ToString()
        {
            if (Negative) return $"!{Value}";
            return $"{Value}";
        }
    }
}
