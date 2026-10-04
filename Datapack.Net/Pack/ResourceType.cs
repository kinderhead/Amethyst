using Datapack.Net.Utils;

namespace Datapack.Net.Pack
{
    public abstract class ResourceType(string path, string fileExtension)
    {
        public readonly string FileExtension = fileExtension;
        public readonly string Path = path;

        internal readonly List<Resource> Resources = [];

        public virtual void Build(DP pack)
        {
            foreach (var i in Resources)
            {
                pack.WriteFile(ComputePath(i.ID), i.Build(pack));
            }
        }

        public virtual string ComputePath(NamespacedID id, string extraPath = "") => $"data/{id.Namespace}/{Path}/{extraPath}{id.Path}{FileExtension}";
    }

    public abstract class GenericResourceType(string path, string fileExtension = ".json") : ResourceType(path, fileExtension)
    {
        public void Add(Resource resource) => Resources.Add(resource);
        public void Remove(Resource resource) => Resources.Remove(resource);
    }

    public class Advancements() : GenericResourceType("advancements");

    public class ItemModifiers() : GenericResourceType("item_modifiers");

    public class LootTables() : GenericResourceType("loot_tables");

    public class Predicates() : GenericResourceType("predicates");

    public class Recipes() : GenericResourceType("recipes");

    public class Structures() : GenericResourceType("structures", ".nbt");

    public class ChatType() : GenericResourceType("chat_type");

    public class DamageType() : GenericResourceType("damage_type");

    public class DimensionResource() : GenericResourceType("dimension");

    public class DimensionType() : GenericResourceType("dimension_type");

    public class Functions() : GenericResourceType("function", ".mcfunction");

    public class ContextFloatProvider() : GenericResourceType("context_float_provider");

    public class ContextIntProvider() : GenericResourceType("context_int_provider");
}