using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.NumberProviders
{
    public enum ProviderNumberType
    {
        Int,
        Float
    }

    public abstract class NumberProvider(NamespacedID id) : Resource(id)
    {
        public abstract ProviderNumberType NumberType { get; }
        public abstract NamespacedID Type { get; }

        public virtual JToken Render()
        {
            var obj = InnerRender();
            obj["type"] = Type;
            return obj;
        }

        protected abstract JObject InnerRender();

        protected NumberProvider ToType(NumberProvider input)
        {
            return input.NumberType switch
            {
                ProviderNumberType.Int when NumberType == ProviderNumberType.Float => new FromIntProvider(input, "minecraft:fi"),
                ProviderNumberType.Float when NumberType == ProviderNumberType.Int => new FromFloatProvider(input, "minecraft:ff"),
                _ => input
            };
        }

        public override string Build(DP pack)
        {
            if (ID == new NamespacedID()) throw new("NumberProvider cannot be root without an ID");
            return Render().ToString();
        }

        // Definitely not efficient but I don't really care
        public override int GetHashCode() => Render().ToString().GetHashCode();
    }
}