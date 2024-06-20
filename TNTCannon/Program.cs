using Datapack.Net;
using Datapack.Net.CubeLib;
using Datapack.Net.Function;
using Datapack.Net.Pack;
using Datapack.Net.Utils;

namespace TNTCannon
{
    internal class Program
    {
        static void Main()
        {
            var pack = new DP("Wow", "test.zip");

            Project.Settings.VerifyMacros = true;
            Project.Settings.EntityCheckPlayer = true;

            var proj = Project.Create<TNTProject>(pack);
            proj.Build();

            pack.Optimize();
            pack.Build();
        }
    }
}
