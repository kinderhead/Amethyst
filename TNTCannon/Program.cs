using Datapack.Net;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
	internal class Program
	{
		private static void Main()
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
