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

            //var p = Alloc<int>();
            //p.Set(5);
            //Print(p);

            Print("Allocating first object");
            var obj = AllocObj<Funny>();
            obj.Prop = 17;
            obj.Str = "How?";

            Print("Allocating second object");
            var other = AllocObj<Funny>();
            other.Prop = 5;
            other.Str = "Thingy";

            obj.Other = other;

            Print("Allocating temp object");
            var tmp = AllocObj<Funny>();
            obj.Other.Copy(tmp);
            Testy(tmp);
            tmp.Free();

            obj.Say();
        }

        protected override void Tick()
        {

        }

        [DeclareMC("testy")]
        private void _Testy(HeapPointer<Funny> obj)
        {
            Print(obj);
        }

        /// <summary>
        /// Test
        /// </summary>
        [DeclareMC("reset")]
        private void _Reset()
        {
            Heap.Clear();
        }
    }
}
