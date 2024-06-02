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

            var obj = Alloc<Funny>(Local());
            obj.Value = 17;
            Print(obj.Value);
        }

        protected override void Tick()
        {

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
