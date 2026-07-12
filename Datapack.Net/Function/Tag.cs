using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;

namespace Datapack.Net.Function
{
	public class Tag(NamespacedID id, string type) : JsonResource(id)
	{
		[JsonProperty("replace")] public bool Replace;

		[JsonIgnore] public string TagType = type;

		[JsonProperty("values")] public List<NamespacedID> Values = [];
	}
}