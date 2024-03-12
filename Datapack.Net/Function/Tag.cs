using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class Tag(NamespacedID id, string type) : JsonResource(id)
    {
        [JsonProperty("replace")]
        public bool Replace = false;

        [JsonProperty("values")]
        public List<NamespacedID> Values = [];

        [JsonIgnore]
        public string TagType = type;
    }
}
