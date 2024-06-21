using Datapack.Net;
using Datapack.Net.CubeLib;
using Datapack.Net.CubeLib.Builtins;
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

        private Entity Player;

        protected override void Init()
        {
            //RegisterObject<Funny>();
            Player = GlobalEntityRef(new TargetSelector(TargetType.p));
        }

        protected override void Main()
        {
            Print("Running");

            Heap.Clear();

            Player.Health = 10;

            Print((Player.Health + 69) / 4);
        }

        [DeclareMC("boom", ["count"])]
        private void _Boom()
        {
            var max = Local();
            max.DynSet("count");

            var self = EntityRef(TargetSelector.Self);
            self.Teleport(new Position(~new Coord(0), ~new Coord(.5), ~new Coord(0)));

            For(1, max, i =>
            {
                Summon(Entities.Tnt, new Position(~new Coord(0), ~new Coord(-.5), ~new Coord(0)));
            });
        }

        protected override void Tick()
        {
            As(new TargetSelector(TargetType.e, type: Entities.Arrow), i =>
            {
                i.As(() => Summon(Entities.Tnt).SetNBT("fuse", 100));
            });

            AddCommand(new KillCommand(new TargetSelector(TargetType.e, type: Entities.Arrow, nbt: new NBTCompound() { ["inGround"] = true })));
        }
    }
}
