using Datapack.Net.CubeLib;
using Datapack.Net.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNTCannon
{
    public class TNTProject() : Project("tnt")
    {
        private Score Test;

        protected override void Init()
        {
            Test = new("test", "dummy");
            AddScore(Test);
        }

        protected override void Main()
        {
            Print("hello");

            var x = new ScoreRef(Test, ScoreEntity);
            x.Set(5);
            x.Mul(5);
        }
    }
}
