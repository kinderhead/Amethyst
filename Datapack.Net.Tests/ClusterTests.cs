using Amethyst.PM;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.Tests
{
    public class ClusterTests
    {
        [Test]
        public void FilePathResolution()
        {
            var cluster = Cluster.New("test/folder/cluster.json");
            using var _ = Assert.EnterMultipleScope();
            Assert.That(cluster.ResolvePath("../test.json"), Is.EqualTo($"test{Path.PathSeparator}test.json"));

            if (OperatingSystem.IsWindows()) Assert.That(cluster.ResolvePath("C:\\Test.json"), Is.EqualTo("C:\\Test.json"));
            else Assert.That(cluster.ResolvePath("/test.json"), Is.EqualTo("/test.json"));
        }

        [Test]
        public void UrlResolution()
        {
            var cluster = Cluster.New("https://www.test.com/path/cluster.json");
            using var _ = Assert.EnterMultipleScope();
            Assert.That(cluster.ResolvePath("../test.json"), Is.EqualTo("https://www.test.com/test.json"));
            Assert.That(cluster.ResolvePath("http://www.othertest.com/test.json"), Is.EqualTo("http://www.othertest.com/test.json"));
        }

        [Test]
        public void Add()
        {
            var cluster = Cluster.New("cluster.json");
            cluster.Add("1.0.1", new(121, 0), "link.zip", new() { ["minecraft"] = "26.3" });
            cluster.Add("1.0.2", new(122, 0), "link2.zip", new() { ["minecraft"] = "26.4" });

            Assert.That(cluster.Serialize(), Is.EqualTo("""
                                                        {
                                                            "@versions": {
                                                                "1.0.1": {
                                                                    "pack_format": "121.0",
                                                                    "link": "link.zip",
                                                                    "dependencies": {
                                                                        "minecraft": "26.3"
                                                                    }
                                                                },
                                                                "1.0.2": {
                                                                    "pack_format": "122.0",
                                                                    "link": "link2.zip",
                                                                    "dependencies": {
                                                                        "minecraft": "26.4"
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        """.Trim()));
        }

        [Test]
        public void QueryVersions()
        {
            var cluster = Get();
            using var _ = Assert.EnterMultipleScope();

            Assert.That(cluster.GetVersions("minecraft"), Is.EquivalentTo([new MinecraftVersion("26.3")]));
            Assert.That(cluster.GetVersions("utils:math"), Is.EquivalentTo([new MinecraftVersion("1.0.1")]));
            Assert.That(cluster.GetVersions("utils:path"), Is.EquivalentTo([new MinecraftVersion("2.3"), new("2.4")]));
            Assert.That(cluster.GetVersions("utils:path/join"), Is.EquivalentTo([new MinecraftVersion("3.2.8")]));
        }

        [Test]
        public void Query()
        {
            var cluster = Get();
            var entry = cluster.Query("utils:path/join", "3.2.8");
            using var _ = Assert.EnterMultipleScope();

            Assert.That(entry.Format, Is.EqualTo(new PackFormat(121, 0)));
            Assert.That(entry.Link, Is.EqualTo("https://some.other.link.to/path/join/releases/3.2.8.zip"));
            Assert.That(entry.Dependencies, Has.Count.EqualTo(1));
            Assert.That(entry.Dependencies.Single(), Is.EqualTo(new KeyValuePair<string, string>("utils:math", "1.0.1")));
        }

        private static Cluster Get()
        {
            var root = Cluster.From("cluster.json", JObject.Parse("""
                                                                  {
                                                                      "minecraft": "https://www.link.to/minecraft/cluster.json",
                                                                      "utils": "https://some.other.link.to/utils/cluster.json"
                                                                  }
                                                                  """));

            Cluster.AddDownloadCacheEntry("https://www.link.to/minecraft/cluster.json", JObject.Parse("""
                                                                                                      {
                                                                                                          "@versions": {
                                                                                                              "26.3": {
                                                                                                                  "pack_format": "121.0",
                                                                                                                  "link": "https://www.link.to/minecraft/releases/26.3.zip"
                                                                                                              }
                                                                                                          }
                                                                                                      }
                                                                                                      """));

            Cluster.AddDownloadCacheEntry("https://some.other.link.to/utils/cluster.json", JObject.Parse("""
                                                                                                         {
                                                                                                             "math": {
                                                                                                                 "@versions": {
                                                                                                                     "1.0.1": {
                                                                                                                         "pack_format": "121.0",
                                                                                                                         "link": "https://some.other.link.to/utils/releases/1.0.1.zip"
                                                                                                                     }
                                                                                                                 }
                                                                                                             },
                                                                                                             "path": {
                                                                                                                 "@versions": {
                                                                                                                     "2.3": {
                                                                                                                         "pack_format": "121.0",
                                                                                                                         "link": "https://some.other.link.to/path/releases/2.3.zip"
                                                                                                                     },
                                                                                                                     "2.4": {
                                                                                                                         "pack_format": "121.0",
                                                                                                                         "link": "https://some.other.link.to/path/releases/2.4.zip"
                                                                                                                     }
                                                                                                                 },
                                                                                                                 "join": {
                                                                                                                     "@versions": {
                                                                                                                         "3.2.8": {
                                                                                                                             "pack_format": "121.0",
                                                                                                                             "link": "https://some.other.link.to/path/join/releases/3.2.8.zip",
                                                                                                                             "dependencies": {
                                                                                                                                 "utils:math": "1.0.1"
                                                                                                                             }
                                                                                                                         }
                                                                                                                     }
                                                                                                                 }
                                                                                                             }
                                                                                                         }
                                                                                                         """));

            return root;
        }
    }
}