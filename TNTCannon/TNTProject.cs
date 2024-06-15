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

            var list = AllocObj<MCList<NBTString>>();
            list.Add("Bruh");
            list.Add("Bruh2");
            list.Add("Boo");

            Print(list[0]);

            list.ForEach((i, idex) => Print(i));
        }

        protected override void Tick()
        {

        }
    }
}
