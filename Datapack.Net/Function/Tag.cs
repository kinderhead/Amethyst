using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;

namespace Datapack.Net.Function
{
    public class Tag(NamespacedID id, string type) : JsonResource(id)
    {
        [JsonIgnore]
        public readonly string TagType = type;

        [JsonProperty("replace")]
        public bool Replace;

        [JsonProperty("values")]
        public List<NamespacedID> Values = [];
    }
}