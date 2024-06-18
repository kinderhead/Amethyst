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

        private MCList<NBTInt> List;
        private MCBool DoPrint;

        protected override void Init()
        {
            RegisterObject<Funny>();

            List = GlobalAllocObjIfNull<MCList<NBTInt>>();
            DoPrint = new(Global(1));
        }

        protected override void Main()
        {
            Print("Running");
        }

        protected override void Tick()
        {
            If(DoPrint, () => Print(List));
        }

        [DeclareMC("add")]
        private void _Add()
        {
            List.Add(Random((0, 69)));
        }

        [DeclareMC("reset")]
        private void _Reset()
        {
            Heap.Clear();
            DoPrint.Set(false);
        }
    }
}
