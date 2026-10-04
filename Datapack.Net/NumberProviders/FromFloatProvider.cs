using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.NumberProviders
{
    public class FromFloatProvider(NumberProvider input, NamespacedID id) : NumberProvider(id)
    {
        public readonly NumberProvider Input = input;
        public override ProviderNumberType NumberType => ProviderNumberType.Int;
        public override NamespacedID Type => "minecraft:from_float";

        protected override JObject InnerRender() => new() { ["input"] = Input.Render() };
    }
}