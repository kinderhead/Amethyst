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

        ScoreRef Funny;
        ScoreRef Scale;

        protected override void Main()
        {
            //RegisterObject<Funny>();

            Print("Running");
            Heap.Clear();

            Player = GlobalEntityRef(new TargetSelector(TargetType.p));

            Player.As(() =>
            {
                Text = SummonIfDead<TextDisplay>(Global("Boo"));
                Text.SetText(new FormattedText().Text("Boo"));
                Text.SetBillboard(Billboard.Fixed);
            });

            Funny = Global(10);
            Scale = Global(100);
        }

        protected override void Tick()
        {
            If(Scale != new MCRange<int>(100, 1000), () => Funny.Mul(-1));

            Scale.Add(Funny);

            Text.Scale.X.Set(Scale, .01);
        }
    }
}
