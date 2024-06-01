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
            
        }

        protected override void Main()
        {
            Heap.Clear();

            var pointer = Alloc<int>(Local());
            pointer.Set(17);

            Funny(Local(7), pointer);
        }

        protected override void Tick()
        {

        }

        [DeclareMC("func")]
        private void _Funny(ScoreRef x, HeapPointer<int> y)
        {
            Print(x, y.Pointer);
            Print(y);
            Print("boo");
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
