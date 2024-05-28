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
    public class TNTProject() : Project("tnt", new("Wow"))
    {
        protected override void Init()
        {
            
        }

        protected override void Main()
        {
            Local();
            Jump(Test, InternalStorage);
        }

        protected override void Tick()
        {
            
        }

        [DeclareMC("test", macro: true)]
        public void Test()
        {
            AddCommand(new SayCommand("$(register_stack)", true));
        }
    }
}
