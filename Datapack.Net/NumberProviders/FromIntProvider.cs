using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.NumberProviders
{
    public class FromIntProvider(NumberProvider input, NamespacedID id) : NumberProvider(id)
    {
        public readonly NumberProvider Input = input;
        public override ProviderNumberType NumberType => ProviderNumberType.Float;
        public override NamespacedID Type => "minecraft:from_int";

        protected override JObject InnerRender() => new() { ["input"] = Input.Render() };
    }
}