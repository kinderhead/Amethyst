using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
	public class EntityComparison(Entity entity, TargetSelector comparison) : Conditional
	{
		public readonly Entity Entity = entity;
		public readonly TargetSelector Comparison = comparison;

		public override Execute Process(Execute cmd, int tmp = 0)
		{
			var branch = If ? cmd.If : cmd.Unless;

			if (Entity.IsCurrentTarget())
			{
				branch.Entity(Comparison);
			}
			else
			{
				var cur = Project.ActiveProject.EntityRef(TargetSelector.Self);
				Entity.As(cmd);
				branch.Entity(Comparison);
				cur.As(cmd);
			}

			return cmd;
		}
	}

	public class RawEntityComparison(IEntityTarget entity) : Conditional
	{
		public readonly IEntityTarget Entity = entity;

		public override Execute Process(Execute cmd, int tmp = 0)
		{
			var branch = If ? cmd.If : cmd.Unless;

			branch.Entity(Entity);

			return cmd;
		}
	}

	public class EntityExists(Entity entity) : Conditional
	{
		public readonly Entity Entity = entity;

		public override Execute Process(Execute cmd, int tmp = 0)
		{
			var test = Project.ActiveProject.Temp(tmp, 0, "cond");
			Project.ActiveProject.AddCommand(Entity.As(new Execute()).Run(new Scoreboard.Players.Set(test.Target, test.Score, 1)));

			if (If)
			{
				(test == 1).Process(cmd);
			}
			else
			{
				(test != 1).Process(cmd);
			}

			return cmd;
		}
	}

	public static class EntityTargetUtils
	{
		public static RawEntityComparison Exists(this IEntityTarget entity) => new(entity);
	}
}
