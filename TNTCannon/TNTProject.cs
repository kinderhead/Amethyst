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
            //RegisterObject<Funny>();
        }

        protected override void Main()
        {
            Print("Running");

            Heap.Clear();

            var list = AllocObj<MCList<NBTInt>>();

            For(0, Local(69), i => list.Add(i));

            Print(list);

            list.FreeObj();

            //var entity = EntityRef(new TargetSelector(TargetType.p));
            //entity.As(() => AddCommand(new SayCommand("boo")));
        }

        protected override void Tick()
        {
            
        }
    }
}
