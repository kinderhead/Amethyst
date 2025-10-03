using Newtonsoft.Json;

namespace Datapack.Net.Utils
{
	public static class JsonUtils
	{
		public static string Serialize(object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented, new NamespacedIDSerializer());
	}
}
