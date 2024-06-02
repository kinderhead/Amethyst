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
            Heap.Clear();

            var obj = AllocObj<Funny>();
            obj.Value = 17;
            obj.Str = "How?";

            Print(obj.Str);

            var tmp = Alloc<string>();
            obj.Str.Move(tmp);
            Testy(tmp);
            tmp.Free();
        }

        protected override void Tick()
        {

        }

        [DeclareMC("testy")]
        private void _Testy(HeapPointer<string> str)
        {
            Print(str);
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
