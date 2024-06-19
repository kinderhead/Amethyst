using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Pack;
using Newtonsoft.Json.Linq;
using static Datapack.Net.Function.Commands.Execute.Subcommand;

namespace Datapack.Net
{
    public class DP
    {
        protected readonly List<ResourceType> types = [];

        private FileStream? fileStream;
        private ZipArchive? zipFile;

        public readonly string Description;
        public readonly int PackFormat;
        public readonly string FilePath;

        public DP(string description, string filepath, int packFormat = 26)
        {
            types.Add(new Advancements());
            types.Add(new ItemModifiers());
            types.Add(new LootTables());
            types.Add(new Predicates());
            types.Add(new Recipes());
            types.Add(new Structures());
            types.Add(new ChatType());
            types.Add(new DamageType());
            types.Add(new Tags());
            types.Add(new DimensionResource());
            types.Add(new DimensionType());
            types.Add(new Functions());

            Description = description;
            PackFormat = packFormat;
            FilePath = filepath;
        }

        public void Build()
        {
            using (fileStream = new FileStream(FilePath, FileMode.Create))
            {
                using (zipFile = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    foreach (var type in types)
                    {
                        type.Build(this);
                    }

                    WriteFile("pack.mcmeta", new JObject(
                        new JProperty("pack", new JObject(
                            new JProperty("description", Description),
                            new JProperty("pack_format", PackFormat)
                        ))
                    ).ToString());
                }
            }
        }

        public T GetResource<T>() where T : ResourceType
        {
            var type = types.Find(i => i is T);
            if (type != null) return (T)type;
            throw new FileNotFoundException("Resource was not found");
        }

        public void Optimize()
        {
            EmptyFunctions();
            EmptyFunctions(); // 2nd pass
            EmptyFunctions(); // 3rd pass
        }

        private void EmptyFunctions()
        {
            var toRemove = new List<MCFunction>();

            foreach (var i in GetResource<Functions>().Resources.Cast<MCFunction>())
            {
                if (i.Length == 0) toRemove.Add(i);
            }

            foreach (var i in toRemove)
            {
                GetResource<Functions>().Resources.Remove(i);
                Console.WriteLine($"Removing empty function: {i.ID}");
            }

            foreach (var i in GetResource<Functions>().Resources.Cast<MCFunction>())
            {
                for (int e = 0; e < i.Length; e++)
                {
                    var remove = false;
                    if (i.Commands[e] is FunctionCommand cmd && toRemove.Contains(cmd.Function)) remove = true;
                    else if (i.Commands[e] is ReturnCommand ret && ret.Cmd is FunctionCommand retfunc && toRemove.Contains(retfunc.Function))
                    {
                        i.Commands[e] = new ReturnCommand();
                    }
                    else if (i.Commands[e] is Execute ex && ex.Get<Run>().Command is FunctionCommand exfunc && toRemove.Contains(exfunc.Function)) remove = true;

                    if (remove)
                    {
                        i.Commands.RemoveAt(e);
                        e--;
                    }
                }
            }

            foreach (var i in GetResource<Tags>().Resources.Cast<Tag>())
            {
                for (int e = 0; e < i.Values.Count; e++)
                {
                    if (toRemove.FindIndex((a) => a.ID == i.Values[e]) != -1)
                    {
                        i.Values.RemoveAt(e);
                        e--;
                    }
                }
            }
        }

        private readonly List<string> FilesWriten = [];
        internal void WriteFile(string path, string content)
        {
            Console.WriteLine($"Writing to file \"{path}\":\n{content}\n");

            if (zipFile != null)
            {
                if (FilesWriten.Contains(path)) throw new Exception($"Datapack has duplicate file: {path}");

                var entry = zipFile.CreateEntry(path);

                using var stream = new StreamWriter(entry.Open());
                stream.Write(content);
                FilesWriten.Add(path);
            }
            else
            {
                throw new FileNotFoundException("Not generating datapack yet");
            }
        }
    }
}
