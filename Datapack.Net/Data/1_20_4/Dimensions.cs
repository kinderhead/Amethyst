using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data._1_20_4
{
    public static class Dimensions
    {
        public static readonly Dimension Overworld = new(new("minecraft:overworld"));
        public static readonly Dimension Nether = new(new("minecraft:the_nether"));
        public static readonly Dimension End = new(new("minecraft:the_end"));
    }
}
