using Datapack.Net.CubeLib;
using Datapack.Net.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNTCannon
{
    public class TNTProject() : Project("tnt", new("Wow"))
    {
        protected override void Init()
        {

        }

        protected override void Main()
        {
            var x = Local(69);
            x.Mul(420);
            Print(x);
        }
    }
}
