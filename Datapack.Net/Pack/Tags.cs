using Datapack.Net.Function;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Pack
{
    public class Tags : ResourceType
    {
        public Tags() : base("tags", ".json")
        {
            
        }

        public Tag AddTag(Tag tag)
        {
            Resources.Add(tag);
            return tag;
        }

        public Tag GetTag(NamespacedID id, string type)
        {
            return (Tag?)Resources.Find(i => i.ID == id) ?? AddTag(new(id, type));
        }

        public override void Build(DP pack)
        {
            foreach (Tag i in Resources.Cast<Tag>())
            {
                pack.WriteFile(ComputePath(i.ID, i.TagType + "/"), i.Build(pack));
            }
        }
    }
}
