using Datapack.Net;
using Datapack.Net.CubeLib;
using Datapack.Net.CubeLib.Builtins;
using Datapack.Net.CubeLib.EntityWrappers;
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

        TextDisplay Text;
        Entity Player;

        protected override void Main()
        {
            //RegisterObject<Funny>();

            Print("Running");
            Heap.Clear();

            var x = Local(5);
            var y = Local(8);

            Print(x + y);

            var obj = AllocObj<Funny>();
            obj.Say();

            // Player = GlobalEntityRef(new TargetSelector(TargetType.p));

            // Player.As(() =>
            // {
            //     Text = SummonIfDead<TextDisplay>(Global("Boo"));
            //     Text.SetText(new FormattedText().Text("Boo"));
            //     Text.SetBillboard(Billboard.Fixed);
            //     Text.InterpolationDuration = 100;
            //     Text.StartAnimation();
            //     Text.Scale = new(1, 5, 1);
            // });
        }

        protected override void Tick()
        {
            
        }
    }
}
