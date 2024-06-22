using Datapack.Net;
using Datapack.Net.CubeLib;
using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.Data;
using Datapack.Net.Data._1_20_4;
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

        private Funny Obj;

        protected override void Main()
        {
            Print("Running");

            Heap.Clear();

            Obj = AllocObj<Funny>(Global());
            Obj.Prop = 4;
            Obj.Other = AllocObj<Funny>();
            Obj.Other.Str = "Booooooooo";
        }

        [DeclareMC("test")]
        private void _Test()
        {
            Print(Obj);
            Print(Obj.Other);
        }
    }
}
