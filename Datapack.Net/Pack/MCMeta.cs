using Datapack.Net.Function;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.Pack
{
	public class MCMeta
	{
		public FormattedText Description = new FormattedText().Text("A data pack built with Datapack.Net");
		public PackVersion MinVersion = PackVersion.Latest;
		public PackVersion MaxVersion = PackVersion.Latest;

		public MCMeta SetMinVersion(PackVersion min)
		{
			MinVersion = min;
			return this;
		}

		public MCMeta SetMaxVersion(PackVersion max)
		{
			MaxVersion = max;
			return this;
		}

		public MCMeta SetDescription(FormattedText text)
		{
			Description = text;
			return this;
		}

		public MCMeta SetDescription(string text)
		{
			Description = new FormattedText().Text(text);
			return this;
		}

		public JObject Build()
		{
			if (MinVersion > MaxVersion)
			{
				(MinVersion, MaxVersion) = (MaxVersion, MinVersion);
			}

			var ret = new JObject();

			var pack = new JObject
			{
				["description"] = Description.Optimize().ToJson()
			};

			if (MaxVersion.IsNewStyle)
			{
				pack["max_format"] = MaxVersion.Get();
				pack["min_format"] = MinVersion.Get();
			}

			if (!MinVersion.IsNewStyle)
			{
				pack["pack_format"] = MaxVersion.Major;
				pack["supported_formats"] = new JArray(MinVersion, MaxVersion);
			}

			ret["pack"] = pack;
			return ret;
		}
	}
}
