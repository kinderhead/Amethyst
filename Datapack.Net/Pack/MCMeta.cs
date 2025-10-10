using Datapack.Net.Function;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.Pack
{
	public class MCMeta
	{
		public FormattedText Description = new FormattedText().Text("A data pack built with Datapack.Net");
		public PackVersion Min = PackVersion.Latest;
		public PackVersion Max = PackVersion.Latest;

		public MCMeta SetMin(PackVersion min)
		{
			Min = min;
			return this;
		}

		public MCMeta SetMax(PackVersion max)
		{
			Max = max;
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
			if (Min > Max)
			{
				(Min, Max) = (Max, Min);
			}

			var ret = new JObject();

			var pack = new JObject
			{
				["description"] = Description.Optimize().ToJson()
			};

			if (Max.IsNewStyle)
			{
				pack["max_format"] = Max.Get();
				pack["min_format"] = Min.Get();
			}

			if (!Min.IsNewStyle)
			{
				pack["pack_format"] = Max.Major;
				pack["supported_formats"] = new JArray(Min, Max);
			}

			ret["pack"] = pack;
			return ret;
		}
	}
}
