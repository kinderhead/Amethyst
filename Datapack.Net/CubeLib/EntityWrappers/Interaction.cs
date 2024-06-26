using System;
using Datapack.Net.Data;
using Datapack.Net.Data._1_20_4;

namespace Datapack.Net.CubeLib.EntityWrappers
{
    public class Interaction(ScoreRef id) : EntityWrapper(id)
    {
        public FloatEntityProperty Width { get => GetAs<FloatEntityProperty>("width"); set => value.Set(this, "width"); }
        public FloatEntityProperty Height { get => GetAs<FloatEntityProperty>("height"); set => value.Set(this, "height"); }

        public override EntityType Type => Entities.Interaction;

        public override void PlayerCheck()
        {
            
        }
    }
}
