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

        Interaction Test;
        Entity Player;
        ScoreRef Multiplier;

        protected override void Main()
        {
            //RegisterObject<Funny>();

            Print("Running");
            Heap.Clear();

            Player = GlobalEntityRef(new TargetSelector(TargetType.p));

            Player.As(() =>
            {
                Test = SummonIfDead<Interaction>(Global("Boo"));
            });

            Multiplier = Global(1);
            Test.Width = 1;
            Test.Height = 1;
            Test.As(() => Test.Teleport(new Position("~", 100, "~")));
        }

        protected override void Tick()
        {
            If(Test.Width > 100, () => Multiplier.Set(-1))
            .ElseIf(Test.Width <= 1, () => Multiplier.Set(1));

            Test.Width += Multiplier;
            Test.Height += Multiplier;
        }
    }
}
