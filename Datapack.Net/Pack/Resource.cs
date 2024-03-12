using Datapack.Net.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Pack
{
    public abstract class Resource(NamespacedID id)
    {
        [JsonIgnore]
        public readonly NamespacedID ID = id;

        public abstract string Build(Datapack pack);
    }

    public class TextResource(NamespacedID id, string content) : Resource(id)
    {
        public readonly string Content = content;

        public override string Build(Datapack pack)
        {
            return Content;
        }
    }

    public class JsonResource(NamespacedID id) : Resource(id)
    {
        public override string Build(Datapack pack)
        {
            return JsonUtils.Serialize(this);
        }
    }
}
