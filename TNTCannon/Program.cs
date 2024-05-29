using Datapack.Net;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pack = new DP("Wow");
            Project.Create<TNTProject>(pack).Build();
            pack.Optimize();
            pack.Build();
        }
    }
}
