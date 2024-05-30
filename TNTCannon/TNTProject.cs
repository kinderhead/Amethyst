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

        private HeapPointer Object;

        protected override void Init()
        {
            
        }

        protected override void Main()
        {
            Object = AllocIfNull(Global(), 17);
        }

        protected override void Tick()
        {
            var x = Local(5);
            x.Mul(Object.Dereference());
            Print(x);
        }

        [DeclareMC("set")]
        public void Set()
        {
            Object.Set(7);
        }

        [DeclareMC("free")]
        public void Free()
        {
            Object.Free();
        }

        [DeclareMC("reset")]
        public void Reset()
        {
            Heap.Clear();
        }
    }
}
