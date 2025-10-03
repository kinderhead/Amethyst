using Datapack.Net.Utils;
using System.Text;

namespace Datapack.Net.Function.Commands
{
	public class DamageCommand : Command
	{
		public readonly IEntityTarget Target;
		public readonly int Amount;
		public readonly NamespacedID? DamageType;
		public readonly Position? Position;
		public readonly IEntityTarget? By;
		public readonly IEntityTarget? Cause;

		public DamageCommand(IEntityTarget target, int amount, NamespacedID? damageType = null, bool macro = false) : base(macro)
		{
			Target = target.RequireOne();
			Amount = amount;
			DamageType = damageType;
		}

		public DamageCommand(IEntityTarget target, int amount, NamespacedID damageType, Position pos, bool macro = false) : base(macro)
		{
			Target = target.RequireOne();
			Amount = amount;
			DamageType = damageType;
			Position = pos;
		}

		public DamageCommand(IEntityTarget target, int amount, NamespacedID damageType, IEntityTarget by, IEntityTarget? cause = null, bool macro = false) : base(macro)
		{
			Target = target.RequireOne();
			Amount = amount;
			DamageType = damageType;
			By = by.RequireOne();
			Cause = cause?.RequireOne();
		}

		protected override string PreBuild()
		{
			var cmd = new StringBuilder($"damage {Target.Get()} {Amount} {DamageType?.ToString() ?? ""}");
			if (Position is null && By is null)
			{
				return cmd.ToString();
			}
			else if (Position is not null)
			{
				_ = cmd.Append($" at {Position}");
			}
			else if (By is not null)
			{
				_ = cmd.Append($" by {By.Get()}{(Cause is null ? "" : $" from {Cause.Get()}")}");
			}

			return cmd.ToString();
		}
	}
}
