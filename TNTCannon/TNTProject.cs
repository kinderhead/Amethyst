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
        ScoreRef Left;
        ScoreRef Right;

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

            Left = Global(0);
            Right = Global(0);
        }

        protected override void Tick()
        {
            If(Test.Width != new MCRange<int>(1, 100), () => Multiplier.Mul(-1));

            Test.Width += Multiplier;
            Test.Height += Multiplier;

            Test.Attacker.As(() =>
            {
                Left.Add(1);
                Test.ClearAttacker();
            });

            Test.Interactor.As(() =>
            {
                Right.Add(1);
                Test.ClearInteractor();
            });

            Printer();
        }

        [DeclareMC("printer")]
        private void _Printer()
        {
            Print("Left clicks:", Left, "Right clicks:", Right);
        }
    }
}
