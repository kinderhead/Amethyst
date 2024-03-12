using Datapack.Net.Function;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Data
{
    public class EntityType(NamespacedID id) : INegatable<EntityType>
    {
        public readonly NamespacedID ID = id;

        public override string ToString()
        {
            return ID.ToString();
        }

        public Negatable<EntityType> Negate()
        {
            return new(this, true);
        }

        public static Negatable<EntityType> operator !(EntityType type) => type.Negate();
        public static implicit operator Negatable<EntityType>(EntityType type) => new(type);
    }
}
