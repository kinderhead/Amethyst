using Datapack.Net;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pack = new DP("Wow");

            var proj = Project.Create<TNTProject>(pack);
            //proj.ErrorChecking = true;
            proj.Build();

            pack.Optimize();
            pack.Build();
        }
    }
}
