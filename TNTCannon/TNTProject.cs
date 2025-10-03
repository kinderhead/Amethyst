using Datapack.Net;
using Datapack.Net.CubeLib;

namespace TNTCannon
{
	[Project]
	public partial class TNTProject(DP pack) : Project(pack)
	{
		public override string Namespace => "tnt";

		//TextDisplay Text;
		//Entity Player;

		protected override void Main()
		{
			//RegisterObject<Funny>();

			Print("Running");
			Heap.Clear();

			var x = Local(5);
			var y = Local(8);

			Print(x + y);

			var obj = AllocObj<Funny>();
			obj.Say();

			// Player = GlobalEntityRef(new TargetSelector(TargetType.p));

			// Player.As(() =>
			// {
			//     Text = SummonIfDead<TextDisplay>(Global("Boo"));
			//     Text.SetText(new FormattedText().Text("Boo"));
			//     Text.SetBillboard(Billboard.Fixed);
			//     Text.InterpolationDuration = 100;
			//     Text.StartAnimation();
			//     Text.Scale = new(1, 5, 1);
			// });
		}

		protected override void Tick()
		{

		}
	}
}
