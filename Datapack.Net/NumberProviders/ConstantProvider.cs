using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.NumberProviders
{
    public class ConstantProvider(int val, NamespacedID id) : NumberProvider(id)
    {
        public readonly int Value = val;
        public override ProviderNumberType NumberType => ProviderNumberType.Int;
        public override NamespacedID Type => "minecraft:constant";

        public override JToken Render() => Value;
        protected override JObject InnerRender() => new() { ["value"] = Value };
    }
}