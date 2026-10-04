using Datapack.Net.NumberProviders;

namespace Datapack.Net.Function.Commands
{
    public class ComputeCommand(NumberProvider provider, bool macro = false) : Command(macro)
    {
        public readonly NumberProvider Provider = provider;

        protected override string PreBuild() => $"compute default {(Provider.NumberType == ProviderNumberType.Int ? "integer" : "float")} {Provider.ID}";
    }
}