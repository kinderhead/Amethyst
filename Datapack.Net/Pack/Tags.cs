using Datapack.Net.Function;
using Datapack.Net.Utils;

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

		public Tag GetTag(NamespacedID id, string type) => (Tag?)Resources.Find(i => i.ID == id) ?? AddTag(new(id, type));

		public override void Build(DP pack)
		{
			foreach (var i in Resources.Cast<Tag>())
			{
				pack.WriteFile(ComputePath(i.ID, i.TagType + "/"), i.Build(pack));
			}
		}
	}
}
