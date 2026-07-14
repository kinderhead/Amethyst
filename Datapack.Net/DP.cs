using System.IO.Compression;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using static Datapack.Net.Function.Commands.Execute.Subcommand;

namespace Datapack.Net
{
    public class DP
    {
        public readonly string FilePath;

        private readonly HashSet<string> filesWriten = [];
        public readonly MCMeta Meta;
        protected readonly List<ResourceType> Types = [];

        private FileStream? fileStream;
        private ZipArchive? zipFile;

        public DP(string filePath, MCMeta meta)
        {
            Types.Add(new Advancements());
            Types.Add(new ItemModifiers());
            Types.Add(new LootTables());
            Types.Add(new Predicates());
            Types.Add(new Recipes());
            Types.Add(new Structures());
            Types.Add(new ChatType());
            Types.Add(new DamageType());
            Types.Add(new Tags());
            Types.Add(new DimensionResource());
            Types.Add(new DimensionType());
            Types.Add(new Functions());

            FilePath = filePath;
            Meta = meta;
        }

        public Functions Functions => GetResource<Functions>();
        public Tags Tags => GetResource<Tags>();

        public void Build()
        {
            using (fileStream = new(FilePath, FileMode.Create))
            {
                using (zipFile = new(fileStream, ZipArchiveMode.Create))
                {
                    foreach (var type in Types)
                    {
                        type.Build(this);
                    }

                    WriteFile("pack.mcmeta", Meta.Build().ToString());
                }
            }
        }

        public T GetResource<T>() where T : ResourceType
        {
            var type = Types.Find(i => i is T);
            if (type != null) return (T)type;

            throw new FileNotFoundException("Resource was not found");
        }

        public void Optimize()
        {
            while (EmptyFunctions())
            {
            }
        }

        private bool EmptyFunctions()
        {
            var toRemove = new List<MCFunction>();

            foreach (var i in GetResource<Functions>().Resources.Cast<MCFunction>())
            {
                if (i.Length == 0) toRemove.Add(i);
            }

            foreach (var i in toRemove)
            {
                GetResource<Functions>().Resources.Remove(i);

#if DEBUG
                Console.WriteLine($"Removing empty function: {i.ID}");
#endif
            }

            foreach (var i in GetResource<Functions>().Resources.Cast<MCFunction>())
            {
                for (var e = 0; e < i.Length; e++)
                {
                    var remove = false;
                    if (i.Commands[e] is FunctionCommand cmd && toRemove.Select(i => i.ID).Contains(cmd.Function))
                        remove = true;
                    else if (i.Commands[e] is ReturnCommand ret && ret.Cmd is FunctionCommand retfunc &&
                             toRemove.Select(i => i.ID).Contains(retfunc.Function))
                        i.Commands[e] = new ReturnCommand();
                    else if (i.Commands[e] is Execute ex && ex.Get<Run>().Command is FunctionCommand exfunc &&
                             toRemove.Select(i => i.ID).Contains(exfunc.Function))
                        remove = true;

                    if (remove)
                    {
                        i.Commands.RemoveAt(e);
                        e--;
                    }
                }
            }

            foreach (var i in GetResource<Tags>().Resources.Cast<Tag>())
            {
                for (var e = 0; e < i.Values.Count; e++)
                {
                    if (toRemove.FindIndex(a => a.ID == i.Values[e]) != -1)
                    {
                        i.Values.RemoveAt(e);
                        e--;
                    }
                }
            }

            return toRemove.Count != 0;
        }

        internal void WriteFile(string path, string content)
        {
//#if DEBUG
//			if (path.EndsWith(".mcfunction"))
//			{
//				Console.WriteLine($"Writing to file \"{path}\":\n{content}\n");
//			}
//#endif

            if (zipFile != null)
            {
                if (filesWriten.Contains(path)) throw new($"Datapack has duplicate file: {path}");

                var entry = zipFile.CreateEntry(path);

                using var stream = new StreamWriter(entry.Open());
                stream.Write(content);
                filesWriten.Add(path);
            }
            else
                throw new FileNotFoundException("Not generating datapack yet");
        }
    }
}