using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.NumberProviders
{
    public abstract class UnaryOperationProvider(NumberProvider input, NamespacedID id) : NumberProvider(id)
    {
        public readonly NumberProvider Input = input;

        public override ProviderNumberType NumberType
        {
            get
            {
                if (SupportsInt && !SupportsFloat)
                    return ProviderNumberType.Int;
                else if (!SupportsInt && SupportsFloat)
                    return ProviderNumberType.Float;
                else
                    return Input.NumberType;
            }
        }
        public abstract bool SupportsInt { get; }
        public abstract bool SupportsFloat { get; }

        protected override JObject InnerRender() => new() { ["input"] = Input.Render() };
    }

    public class AbsProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => true;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:abs";
    }

    public class SqrtProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => false;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:sqrt";
    }

    public class SinProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => false;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:sin";
    }

    public class CosProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => false;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:cos";
    }

    public class RoundProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => false;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:round";
    }

    public class FloorProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => false;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:floor";
    }

    public class CeilProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => false;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:ceil";
    }
    
    public class TruncateProvider(NumberProvider input, NamespacedID id) : UnaryOperationProvider(input, id)
    {
        public override bool SupportsInt => false;
        public override bool SupportsFloat => true;
        public override NamespacedID Type => "minecraft:truncate";
    }
}
