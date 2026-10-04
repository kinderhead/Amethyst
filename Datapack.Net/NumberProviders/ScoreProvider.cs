using Datapack.Net.Function;
using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.NumberProviders
{
    public class ScoreProvider(IEntityTarget target, Score score, NamespacedID id) : NumberProvider(id)
    {
        public readonly Score Score = score;

        public readonly IEntityTarget Target = target;
        public override ProviderNumberType NumberType => ProviderNumberType.Int;
        public override NamespacedID Type => "minecraft:score";

        protected override JObject InnerRender() => new()
        {
            ["target"] = new JObject
            {
                ["type"] = "fixed",
                ["name"] = Target.Get()
            },
            ["score"] = Score.Name
        };
    }
}