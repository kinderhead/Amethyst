using Datapack.Net.Function;
using Datapack.Net.Utils;
using Newtonsoft.Json;

namespace Datapack.Net.Pack
{
    public class Tags() : ResourceType("tags", ".json")
    {
        public Tag AddTag(Tag tag)
        {
            Resources.Add(tag);
            return tag;
        }

        public Tag GetTag(NamespacedID id, string type) => (Tag?)Resources.Find(i => i.ID == id) ?? AddTag(new(id, type));

        public override void Build(DP pack)
        {
            foreach (var i in Resources.Cast<Tag>())
            {
                pack.WriteFile(ComputePath(i.ID, i.TagType + "/"), i.Build(pack));
            }
        }

        public override void Import(DP pack, string ns, string path, string file)
        {
            var pathList = path.Split(PathUtils.Separators, StringSplitOptions.RemoveEmptyEntries);
            if (pathList.Length < 2) base.Import(pack, ns, path, file);
            else
            {
                var id = new NamespacedID(ns, string.Join('.', string.Join('/', pathList[1..]).Split('.')[..^1]));
                var tag = GetTag(id, pathList[0]);
                var newTag = JsonConvert.DeserializeObject<Tag>(file) ?? throw new InvalidOperationException($"Invalid tag {id} in imported pack");

                foreach (var i in newTag.Values)
                {
                    if (!tag.Values.Contains(i)) tag.Values.Add(i);
                }
            }
        }
    }
}