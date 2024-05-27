using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Pack
{
    public abstract class ResourceType(string path, string fileExtension)
    {
        public readonly string Path = path;
        public readonly string FileExtension = fileExtension;

        protected List<Resource> resources = [];

        public virtual void Build(Datapack pack)
        {
            foreach (var i in resources)
            {
                pack.WriteFile(ComputePath(i.ID), i.Build(pack));
            }
        }

        public string ComputePath(NamespacedID id, string extraPath = "")
        {
            return $"data/{id.Namespace}/{Path}/{extraPath}{id.Path}{FileExtension}";
        }
    }

    public abstract class GenericResourceType(string path, string fileExtension = ".json") : ResourceType(path, fileExtension)
    {
        public void Add(Resource resource)
        {
            resources.Add(resource);
        }
    }

    public class Advancements : GenericResourceType
    {
        public Advancements() : base("advancements")
        {
        }
    }

    public class ItemModifiers : GenericResourceType
    {
        public ItemModifiers() : base("item_modifiers")
        {
        }
    }

    public class LootTables : GenericResourceType
    {
        public LootTables() : base("loot_tables")
        {
        }
    }

    public class Predicates : GenericResourceType
    {
        public Predicates() : base("predicates")
        {
        }
    }

    public class Recipes : GenericResourceType
    {
        public Recipes() : base("recipes")
        {
        }
    }

    public class Structures : GenericResourceType
    {
        public Structures() : base("structures", ".nbt")
        {
        }
    }

    public class ChatType : GenericResourceType
    {
        public ChatType() : base("chat_type")
        {
        }
    }

    public class DamageType : GenericResourceType
    {
        public DamageType() : base("damage_type")
        {
        }
    }

    public class DimensionResource : GenericResourceType
    {
        public DimensionResource() : base("dimension")
        {
        }
    }

    public class DimensionType : GenericResourceType
    {
        public DimensionType() : base("dimension_type")
        {
        }
    }

    public class Functions : GenericResourceType
    {
        public Functions() : base("functions", ".mcfunction")
        {
        }
    }
}
