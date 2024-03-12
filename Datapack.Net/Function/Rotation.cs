using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public readonly struct Rotation(RotCoord x, RotCoord y)
    {
        public readonly RotCoord X = x;
        public readonly RotCoord Y = y;

        public override string ToString()
        {
            return $"{X} {Y}";
        }
    }

    public readonly struct RotCoord(double val, RotCoordType type = RotCoordType.Global)
    {
        public readonly RotCoordType Type = type;
        public readonly double Value = val;

        public override string ToString()
        {
            return Type switch
            {
                RotCoordType.Relative => $"~{Value}",
                _ => $"{Value}"
            };
        }

        public static implicit operator RotCoord(double val) => new(val);
        public static RotCoord operator ~(RotCoord coord) => new(coord.Value, RotCoordType.Relative);
    }

    public enum RotCoordType
    {
        Global,
        Relative
    }
}
