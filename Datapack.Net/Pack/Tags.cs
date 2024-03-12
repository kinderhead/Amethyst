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

        public void AddTag(Tag tag)
        {
            resources.Add(tag);
        }

        public Tag GetTag(NamespacedID id)
        {
            var tag = (Tag?)resources.Find(i => i.ID == id);

            if (tag != null) return tag;
            throw new FileNotFoundException($"Tag {id} was not found");
        }

        public override void Build(Datapack pack)
        {
            foreach (Tag i in resources.Cast<Tag>())
            {
                pack.WriteFile(ComputePath(i.ID, i.TagType + "/"), i.Build(pack));
            }
        }
    }
}
