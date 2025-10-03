using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
	public class PointerExists : Conditional
	{
		public IPointer Pointer;

		public override Execute Process(Execute cmd, int tmp = 0)
		{
			var branch = If ? cmd.If : cmd.Unless;

			var tempVar = Project.ActiveProject.Temp(tmp, "cmp");
			Project.ActiveProject.Std.PointerExists([new("pointer", "a"), .. Pointer.StandardMacros()], tempVar);

			branch.Score(tempVar.Target, tempVar.Score, 1);

			return cmd;
		}
	}
}
