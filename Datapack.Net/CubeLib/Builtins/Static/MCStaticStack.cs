using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib.Builtins.Static
{
	public class MCStaticStack(Storage storage, string path) : IStaticType
	{
		public readonly Storage Storage = storage;
		public readonly string Path = path;

		public void Init() => Project.ActiveProject.PrependMain(new DataCommand.Modify(Storage, Path).Set().Value("[]"));

		public void Enqueue(ScoreRef value)
		{
			Project.ActiveProject.AddCommand(new Execute().Store(Project.ActiveProject.InternalStorage, "tmpstack", Data.NBTNumberType.Int, 1).Run(new Scoreboard.Players.Get(value.Target, value.Score)));
			Project.ActiveProject.AddCommand(new DataCommand.Modify(Storage, Path).Insert(0).From(Project.ActiveProject.InternalStorage, "tmpstack"));
		}

		public void Enqueue(object obj) => Project.ActiveProject.Std.StackEnqueue([new("storage", Storage), new("path", Path), new("value", obj)]);

		public void Dequeue(ScoreRef to)
		{
			Project.ActiveProject.AddCommand(new Execute().Store(to.Target, to.Score).Run(new DataCommand.Get(Storage, $"{Path}[0]")));
			Project.ActiveProject.AddCommand(new DataCommand.Remove(Storage, $"{Path}[0]"));
		}

		/// <summary>
		/// Dequeues to <c>InternalStorage.tmpstack</c>.
		/// </summary>
		public void DequeueToStorage()
		{
			Project.ActiveProject.AddCommand(new DataCommand.Modify(Project.ActiveProject.InternalStorage, "tmpstack_st").Set().From(Storage, $"{Path}[0]"));
			Project.ActiveProject.AddCommand(new DataCommand.Remove(Storage, $"{Path}[0]"));
		}

		public ScoreRef Dequeue()
		{
			var score = Project.ActiveProject.Local();
			Dequeue(score);
			return score;
		}
	}
}
