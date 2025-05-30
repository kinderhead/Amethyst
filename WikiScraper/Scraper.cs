using Datapack.Net.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace WikiScraper
{
    public static partial class Scraper
    {
		public static JObject ParseComponent(string comp)
		{
			throw new NotImplementedException();
			//var data = GetRawComponentInfo(comp);
			//var def = GetDef(data[0]);

			//return [];
		}

		public static Definition GetDef(string line)
		{
			var decl = line.Trim([' ', '*']).Replace("minecraft:", "minecraft@").Split(':')[0];

			var nameMatch = BracketNameRegex().Match(decl);
			if (!nameMatch.Success) nameMatch = QuoteNameRegex().Match(decl);
			var name = nameMatch.Groups["name"].Value;
			name = name.Replace("minecraft@", "minecraft:");

			List<Property> types = [];
			foreach (Match i in TypeRegex().Matches(decl))
			{
				var type = i.Groups["type"].Value;
				if (type == "string") types.Add(new StringProperty());
				else if (type == "list") types.Add(new ListProperty());
				else if (type == "compound") types.Add(new CompoundProperty());
				else if (type == "int-array") types.Add(new IntArrayProperty());
				else throw new NotImplementedException();
			}

			return new(Reduce(types, name), name);
		}

		public static Property Reduce(List<Property> types, string name)
		{
			throw new NotImplementedException();
			//if (types.Count == 1) return types.First();
		}

        public static string[] GetRawComponentInfo(string urlname)
        {
            var data = Fetch($"https://minecraft.wiki/w/Data_component_format/{urlname}?action=raw");
			return [.. data.Replace("<onlyinclude>", "").Split('\n').Where(i => i.StartsWith('*'))];
        }

        public static string Fetch(string url)
        {
            using var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, url)
			{
				Version = new(2, 0)
			};

			request.Headers.Clear();
			request.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
			request.Headers.Add("accept-language", "en-US,en;q=0.9,fr;q=0.8");
			request.Headers.Add("priority", "u=0, i");
			request.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"134\", \"Not:A-Brand\";v=\"24\", \"Google Chrome\";v=\"134\"");
			request.Headers.Add("sec-ch-ua-mobile", "?0");
			request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
			request.Headers.Add("sec-fetch-dest", "document");
			request.Headers.Add("sec-fetch-mode", "navigate");
			request.Headers.Add("sec-fetch-site", "none");
			request.Headers.Add("sec-fetch-user", "?1");
			request.Headers.Add("upgrade-insecure-requests", "1");
			request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36");

			HttpResponseMessage response = client.SendAsync(request).Result;
			response.EnsureSuccessStatusCode();
			return response.Content.ReadAsStringAsync().Result;
		}

		[GeneratedRegex(@"{{nbt\|\w*\|(?<name>(\w|@)*)}}")]
		private static partial Regex BracketNameRegex();

		[GeneratedRegex(@"'''(?<name>(\w|@)*)'''")]
		private static partial Regex QuoteNameRegex();

		[GeneratedRegex(@"{{nbt\|(?<type>(\w|-)*)(\|\w*)?}}")]
		private static partial Regex TypeRegex();
	}
}
