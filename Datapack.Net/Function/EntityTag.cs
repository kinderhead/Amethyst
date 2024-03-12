using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class EntityTag(string name) : INegatable<EntityTag>
    {
        public readonly string Name = name;

        public Negatable<EntityTag> Negate()
        {
            return new(this, true);
        }

        public override string ToString()
        {
            return Name;
        }

        public static Negatable<EntityTag> operator !(EntityTag entityTag) => entityTag.Negate();
        public static implicit operator Negatable<EntityTag>(EntityTag entityTag) => new(entityTag);
    }
}
