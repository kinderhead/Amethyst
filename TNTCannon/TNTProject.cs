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
        private ScoreRef Counter;

        protected override void Init()
        {
            Counter = Global(0);
        }

        protected override void Main()
        {
            Print("boo");
        }

        protected override void Tick()
        {
            
        }
    }
}
