using Datapack.Net;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pack = new DP("Wow");

            Project.Settings.VerifyMacros = true;

            var proj = Project.Create<TNTProject>(pack);
            proj.Build();

            pack.Optimize();
            pack.Build();
        }
    }
}
