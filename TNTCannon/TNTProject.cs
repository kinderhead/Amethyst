using Datapack.Net;
using Datapack.Net.CubeLib;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNTCannon
{
    public class TNTProject(DP pack) : Project(pack)
    {
        public override string Namespace => "tnt";

        protected override void Init()
        {
            
        }

        protected override void Main()
        {
            Call(Std.Test);
            var x = Local();
            CallRet(Std.AllocAddress, x, [new("storage", Heap.Storage.ToString()), new("path", Heap.Path)]);
            Print(x);
        }

        protected override void Tick()
        {
            
        }
    }
}
