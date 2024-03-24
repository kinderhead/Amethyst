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

        public Swizzle(bool x, bool y, bool z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Swizzle(string key)
        {
            key = key.ToLower();
            X = key.Contains('x');
            Y = key.Contains('y');
            Z = key.Contains('z');
        }

        public override string ToString()
        {
            return $"{(X ? "x": "")}{(Y ? "y" : "")}{(Z ? "z" : "")}";
        }
    }
}
