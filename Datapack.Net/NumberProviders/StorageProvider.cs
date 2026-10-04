using Datapack.Net.Function;
using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.NumberProviders
{
    public class StorageProvider(IDataTarget target, NamespacedID id) : NumberProvider(id)
    {
        public readonly IDataTarget Target = target;
        public override ProviderNumberType NumberType => ProviderNumberType.Int;
        public override NamespacedID Type => "minecraft:storage";

        protected override JObject InnerRender() => new()
        {
            ["storage"] = Target.Source,
            ["path"] = Target.Path
        };
    }
}