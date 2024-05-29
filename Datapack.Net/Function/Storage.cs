using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class Storage(NamespacedID id)
    {
        public readonly NamespacedID ID = id;

        public override string ToString() => ID.ToString();
    }

    public class StorageMacro(string raw) : Storage(new())
    {
        public readonly string Value = raw;

        public override string ToString() => Value;
    }
}
