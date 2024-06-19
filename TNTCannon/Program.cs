using Datapack.Net;
using Datapack.Net.CubeLib;
using Datapack.Net.Function;
using Datapack.Net.Pack;
using Datapack.Net.Utils;

namespace TNTCannon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pack = new DP("Wow", "test.zip");

            Project.Settings.VerifyMacros = true;

            var proj = Project.Create<TNTProject>(pack);
            proj.Build();

            pack.Optimize();
            pack.Build();
        }
    }
}
