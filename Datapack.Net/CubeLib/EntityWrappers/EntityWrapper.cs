using System;
using Datapack.Net.Data;

namespace Datapack.Net.CubeLib.EntityWrappers
{
    public abstract class EntityWrapper(ScoreRef id) : Entity(id)
    {
        public abstract EntityType Type { get; }

        public static T Create<T>(Entity entity) where T : EntityWrapper => (T?)Activator.CreateInstance(typeof(T), [entity.ID]) ?? throw new ArgumentException("Could not create entity");
    }
}
