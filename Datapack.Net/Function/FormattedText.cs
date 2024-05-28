using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function
{
    public class FormattedText
    {
        private readonly JArray Obj = [];

        public FormattedText Text(string str)
        {
            Obj.Add(new JObject(new JProperty("text", str)));
            return this;
        }

        public FormattedText Score(IEntityTarget target, Score score)
        {
            Obj.Add(new JObject(new JProperty("score", new JObject(new JProperty("name", target.RequireOne().Get()), new JProperty("objective", score.Name)))));
            return this;
        }

        public void RemoveLast() => Obj.RemoveAt(Obj.Count - 1);

        public override string ToString()
        {
            return Obj.ToString(Newtonsoft.Json.Formatting.None);
        }
    }
}
