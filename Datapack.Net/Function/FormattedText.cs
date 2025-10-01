using Newtonsoft.Json.Linq;

namespace Datapack.Net.Function
{
    public class FormattedText
    {
        private readonly JArray Obj = [];
        public bool HasHoverOrClickEvents = false;
        public bool Macro = false;
        public int Count => Obj.Count;

        private readonly Stack<Modifiers> ModifierStack = [];

        public FormattedText PushModifiers(Modifiers mod)
        {
            ModifierStack.Push(mod);
            return this;
        }

        public FormattedText PopModifiers()
        {
            ModifierStack.Pop();
            return this;
        }

        public FormattedText Text(string str, Modifiers? modifiers = null)
        {
            modifiers ??= ModifierStack.Count != 0 ? ModifierStack.Peek() : null;
            if (modifiers is null) Obj.Add(str);
            else
            {
                Obj.Add(modifiers.Value.Process(new JObject(new JProperty("text", str)), this));
            }

            return this;
        }

        public FormattedText Score(IEntityTarget target, Score score, Modifiers? modifiers = null)
        {
            modifiers ??= ModifierStack.Count != 0 ? ModifierStack.Peek() : new();
            Obj.Add(modifiers.Value.Process(new JObject(new JProperty("score", new JObject(new JProperty("name", target.RequireOne().Get()), new JProperty("objective", score.Name)))), this));
            return this;
        }

        public FormattedText NBT(IDataTarget target, Modifiers? modifiers = null)
        {
            modifiers ??= ModifierStack.Count != 0 ? ModifierStack.Peek() : new();
            Obj.Add(modifiers.Value.Process(new JObject(new JProperty("nbt", target.Path), new JProperty(target.Type, target.Source)), this));
            return this;
        }

        public FormattedText Entity(IEntityTarget target, string sep = ", ", Modifiers? modifiers = null)
        {
            modifiers ??= ModifierStack.Count != 0 ? ModifierStack.Peek() : new();
            Obj.Add(modifiers.Value.Process(new JObject(new JProperty("selector", target.Get()), new JProperty("separator", sep)), this));
            return this;
        }

        public void RemoveLast() => Obj.RemoveAt(Obj.Count - 1);

        public FormattedText Optimize()
        {
            for (int i = 1; i < Obj.Count; i++)
            {
                if (Obj[i - 1] is JValue v1 && v1.Type == JTokenType.String && Obj[i] is JValue v2 && v2.Type == JTokenType.String)
                {
                    v1.Value += (string?)v2.Value;
                    Obj.RemoveAt(i--);
                }
            }

            return this;
        }

        public override string ToString()
        {
            return Obj.ToString(Newtonsoft.Json.Formatting.None);
        }

        public struct Modifiers()
        {
            public string Color = "";
            public FormattedText? HoverText;
            public string? SuggestCommand;

            public bool? Underlined;

            public readonly JObject Process(JObject obj, FormattedText self)
            {
                if (Color != "") obj["color"] = Color;
                if (Underlined is not null) obj["underlined"] = Underlined;

                if (HoverText is not null)
                {
                    if (HoverText.HasHoverOrClickEvents) throw new ArgumentException("Formatted text has hover or click events in invalid places");
                    self.HasHoverOrClickEvents = true;
                    obj["hover_event"] = new JObject { { "action", "show_text" }, { "value", HoverText.Obj } };
                }

                if (SuggestCommand is not null)
                {
                    self.HasHoverOrClickEvents = true;
                    obj["click_event"] = new JObject { { "action", "suggest_command" }, { "command", SuggestCommand } };
                }
                return obj;
            }
        }
    }
}
