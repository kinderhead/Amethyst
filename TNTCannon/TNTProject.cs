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
            
        }

        protected override void Tick()
        {
            Counter.Add(1);
            If(Counter >= 5, () =>
            {
                var mod = Local(Counter);
                mod.Mod(69);
                Print("Woah", Counter, mod);
            });
        }
    }
}
