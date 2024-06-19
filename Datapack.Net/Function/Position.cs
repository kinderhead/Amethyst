using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public readonly struct Position(Coord x, Coord y, Coord z)
    {
        public readonly Coord X = x;
        public readonly Coord Y = y;
        public readonly Coord Z = z;

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }

        public static Position Current => new(new(0, CoordType.Relative), new(0, CoordType.Relative), new(0, CoordType.Relative));
    }

    public readonly struct Coord(double val, CoordType type = CoordType.Global)
    {
        public readonly CoordType Type = type;
        public readonly double Value = val;

        public override string ToString()
        {
            return Type switch
            {
                CoordType.Relative => $"~{Value}",
                CoordType.Local => $"^{Value}",
                _ => $"{Value}",
            };
        }

        public static implicit operator Coord(double val) => new(val);
        public static Coord operator ~(Coord coord) => new(coord.Value, CoordType.Relative);
        public static Coord operator !(Coord coord) => new(coord.Value, CoordType.Local);
    }

    public enum CoordType
    {
        Global,
        Relative,
        Local
    }
}
