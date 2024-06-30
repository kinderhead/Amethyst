using System;
using Datapack.Net.Data;
using Datapack.Net.Data._1_20_4;

namespace Datapack.Net.CubeLib.EntityWrappers
{
    public abstract class EntityWrapper(ScoreRef id) : Entity(id)
    {
        public abstract EntityType Type { get; }

        public override bool NeedsPlayerCheck() => Type == Entities.Player && base.NeedsPlayerCheck();

        public static T Create<T>(Entity entity) where T : EntityWrapper => (T?)Activator.CreateInstance(typeof(T), [entity.ID]) ?? throw new ArgumentException("Could not create entity");

        public static Project State => Project.ActiveProject;
    }
}
