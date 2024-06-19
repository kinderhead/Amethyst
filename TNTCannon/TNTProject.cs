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

        protected override void Init()
        {
            //RegisterObject<Funny>();
        }

        protected override void Main()
        {
            Print("Running");

            Heap.Clear();

            var player = EntityRef(new NamedTarget("kinderhead"));
            player.As(() =>
            {
                Summon(Entities.Chicken, Position.Current).Kill();
            });
        }

        [DeclareMC("do")]
        private void _Do()
        {
            var entity = EntityRef(new NamedTarget("virchelovek"));

            entity.As(() =>
            {
                var dest = EntityRef(new TargetSelector(TargetType.e, type: Entities.Sheep, sort: SortType.Nearest, limit: 1));
                entity.Teleport(dest);
            });
        }

        protected override void Tick()
        {
            //As(new TargetSelector(TargetType.a), (i) =>
            //{
            //    var random = EntityRef(new TargetSelector(TargetType.e, sort: SortType.Random, limit: 1));
            //    i.Teleport(random);
            //});
        }
    }
}
