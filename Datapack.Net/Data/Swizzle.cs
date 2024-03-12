using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public readonly struct Swizzle
    {
        public readonly bool X;
        public readonly bool Y;
        public readonly bool Z;

        public override string ToString()
        {
            return $"{(X ? "X": "")}{(Y ? "X" : "")}{(Z ? "X" : "")}";
        }
    }
}
