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
        public bool HasHoverOrClickEvents = false;

        public FormattedText Text(string str, Modifiers? modifiers = null)
        {
            modifiers ??= new();
            Obj.Add(modifiers.Value.Process(new JObject(new JProperty("text", str)), this));
            return this;
        }

        public FormattedText Score(IEntityTarget target, Score score, Modifiers? modifiers = null)
        {
            modifiers ??= new();
            Obj.Add(modifiers.Value.Process(new JObject(new JProperty("score", new JObject(new JProperty("name", target.RequireOne().Get()), new JProperty("objective", score.Name)))), this));
            return this;
        }

        public FormattedText Storage(Storage target, string path, Modifiers? modifiers = null)
        {
            modifiers ??= new();
            Obj.Add(modifiers.Value.Process(new JObject(new JProperty("nbt", path), new JProperty("storage", target.ToString())), this));
            return this;
        }

        public FormattedText Entity(IEntityTarget target, string sep = ", ", Modifiers? modifiers = null)
        {
            modifiers ??= new();
            Obj.Add(modifiers.Value.Process(new JObject(new JProperty("selector", target.Get()), new JProperty("separator", sep)), this));
            return this;
        }

        public void RemoveLast() => Obj.RemoveAt(Obj.Count - 1);

        public override string ToString()
        {
            return Obj.ToString(Newtonsoft.Json.Formatting.None);
        }

        public struct Modifiers()
        {
            public string Color = "";
            public FormattedText? HoverText;

            public readonly JObject Process(JObject obj, FormattedText self)
            {
                if (Color != "") obj["color"] = Color;
                if (HoverText is not null)
                {
                    if (HoverText.HasHoverOrClickEvents) throw new ArgumentException("Formatted text has hover or click events in invalid places");
                    self.HasHoverOrClickEvents = true;
                    obj["hoverEvent"] = new JObject(new JProperty("action", "show_text"), new JProperty("value", HoverText.Obj));
                }
                return obj;
            }
        }
    }
}
