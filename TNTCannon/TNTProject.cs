using Datapack.Net;
using Datapack.Net.CubeLib;
using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNTCannon
{
    [Project]
    public partial class TNTProject(DP pack) : Project(pack)
    {
        public override string Namespace => "tnt";

        protected override void Init()
        {
            RegisterObject<Funny>();
        }

        protected override void Main()
        {
            Print("Running");

            Heap.Clear();

            var x = Local(10);
            var y = Local(5);
            Print(x * 76 * (y + 2));
        }

        protected override void Tick()
        {

        }
    }
}
