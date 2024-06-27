using System;
using Datapack.Net.Data;

namespace Datapack.Net.CubeLib.EntityProperties
{
    public class EntityStructure<T>() : EntityProperty<T> where T : NBTType
    {
        public R GetProp<R>(string path) where R : EntityProperty, new()
        {
            if (Entity is not null && Path is not null) return Entity.GetAs<R>(Path + path);
            else if (Value is not null) return GetFromValue<R>(path);
            else throw new Exception("Invalid EntityStructure");
        }

        public void SetProp<R>(string path, R value) where R : EntityProperty
        {
            if (Entity is not null && Path is not null) value.Set(Entity, Path + path);
            else throw new NotImplementedException();
        }

        protected R GetFromValue<R>(string path) where R : EntityProperty, new()
        {
            if (Value is null) throw new Exception("Value is null");

            if (Value is NBTCompound comp) return new() { RawValue = comp[path] };
            throw new NotImplementedException();
        }
    }
}
