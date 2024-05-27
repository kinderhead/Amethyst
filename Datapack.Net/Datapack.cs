using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datapack.Net.Pack;

namespace Datapack.Net
{
    public class Datapack
    {
        protected readonly List<ResourceType> types = [];

        private FileStream? fileStream;
        private ZipArchive? zipFile;

        public Datapack()
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
        }

        public void Build()
        {
            using (fileStream = new FileStream(@"test.zip", FileMode.Create))
            {
                using (zipFile = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    foreach (var type in types)
                    {
                        type.Build(this);
                    }
                }
            }
        }

        public T GetResource<T>() where T : ResourceType
        {
            var type = types.Find(i => i is T);
            if (type != null) return (T)type;
            throw new FileNotFoundException("Resource was not found");
        }

        internal void WriteFile(string path, string content)
        {
            Console.WriteLine($"Writing to file \"{path}\":\n{content}\n");

            if (zipFile != null)
            {
                var entry = zipFile.CreateEntry(path);

                using var stream = new StreamWriter(entry.Open());
                stream.Write(content);
            }
            else
            {
                throw new FileNotFoundException("Not generating datapack yet");
            }
        }
    }
}
