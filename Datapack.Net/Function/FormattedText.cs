using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.Function
{
    public class FormattedText
    {
        private readonly Stack<Modifiers> modifierStack = [];
        private readonly JArray obj = [];
        public bool HasHoverOrClickEvents;

        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
        [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
        public bool Macro = false;

        public int Count => obj.Count;

        public FormattedText PushModifiers(Modifiers mod)
        {
            modifierStack.Push(mod);
            return this;
        }

        public FormattedText PopModifiers()
        {
            modifierStack.Pop();
            return this;
        }

        public FormattedText Text(string str, Modifiers? modifiers = null)
        {
            modifiers ??= modifierStack.Count != 0 ? modifierStack.Peek() : null;
            if (modifiers is null)
                obj.Add(str);
            else
                obj.Add(modifiers.Value.Process(new(new JProperty("text", str)), this));

            return this;
        }

        public FormattedText Score(IEntityTarget target, Score score, Modifiers? modifiers = null)
        {
            modifiers ??= modifierStack.Count != 0 ? modifierStack.Peek() : new();
            obj.Add(modifiers.Value.Process(
                new(new JProperty("score",
                    new JObject(new JProperty("name", target.RequireOne().Get()),
                        new JProperty("objective", score.Name)))), this));
            return this;
        }

        public FormattedText NBT(IDataTarget target, bool interpret = false, bool plain = false,
                                 Modifiers? modifiers = null)
        {
            modifiers ??= modifierStack.Count != 0 ? modifierStack.Peek() : new();
            var nbt = new JObject(new JProperty("nbt", target.Path), new JProperty(target.Type, target.Source));

            if (interpret) nbt["interpret"] = true;

            if (plain) nbt["plain"] = true;

            obj.Add(modifiers.Value.Process(nbt, this));
            return this;
        }

        public FormattedText Entity(IEntityTarget target, string sep = ", ", Modifiers? modifiers = null)
        {
            modifiers ??= modifierStack.Count != 0 ? modifierStack.Peek() : new();
            obj.Add(modifiers.Value.Process(
                new(new JProperty("selector", target.Get()), new JProperty("separator", sep)), this));
            return this;
        }

        public void RemoveLast() => obj.RemoveAt(obj.Count - 1);

        public FormattedText Optimize()
        {
            for (var i = 1; i < obj.Count; i++)
            {
                if (obj[i - 1] is JValue v1 && v1.Type == JTokenType.String && obj[i] is JValue v2 &&
                    v2.Type == JTokenType.String)
                {
                    v1.Value += (string?)v2.Value;
                    obj.RemoveAt(i--);
                }
            }

            return this;
        }

        public override string ToString() => ToJson().ToString(Formatting.None);
        public JArray ToJson() => obj;

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
                    obj["hover_event"] = new JObject { { "action", "show_text" }, { "value", HoverText.obj } };
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