using Datapack.Net.Function;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.Pack
{
    public class MCMeta
    {
        public readonly List<PackOverlay> Overlays = [];
        public FormattedText Description = new FormattedText().Text("A data pack built with Datapack.Net");
        public PackFormat MaxVersion = PackFormat.Latest;
        public PackFormat MinVersion = PackFormat.Latest;

        public MCMeta SetMinVersion(PackFormat min)
        {
            MinVersion = min;
            return this;
        }

        public MCMeta SetMaxVersion(PackFormat max)
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

        public MCMeta AddOverlay(string directory, PackFormat min, PackFormat max)
        {
            Overlays.Add(new(directory, min, max));
            return this;
        }

        public JObject Build()
        {
            if (MinVersion > MaxVersion) (MinVersion, MaxVersion) = (MaxVersion, MinVersion);

            var ret = new JObject();

            var pack = new JObject { ["description"] = Description.Optimize().ToJson() };

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

            if (Overlays.Count > 0) ret["overlays"] = new JObject { ["entries"] = new JArray(Overlays.Select(i => i.Build())) };

            return ret;
        }
    }

    public readonly struct PackOverlay(string directory, PackFormat min, PackFormat max)
    {
        public readonly string Directory = directory;
        public readonly PackFormat Min = min;
        public readonly PackFormat Max = max;

        public JToken Build() =>
            new JObject
            {
                ["directory"] = Directory,
                ["min_format"] = Min.Get(),
                ["max_format"] = Max.Get()
            };
    }
}