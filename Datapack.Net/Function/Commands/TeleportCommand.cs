namespace Datapack.Net.Function.Commands
{
	public class TeleportCommand : Command
	{
		public readonly IEntityTarget? Target;
		public readonly IEntityTarget? Destination;
		public readonly Position? Location;
		public readonly Rotation? Rotation;
		public readonly Position? FacingLocation;
		public readonly IEntityTarget? FacingEntity;
		public readonly bool Eyes = false;

		public TeleportCommand(IEntityTarget destination, bool macro = false) : base(macro)
		{
			Destination = destination;
		}

		public TeleportCommand(IEntityTarget target, IEntityTarget destination, bool macro = false) : base(macro)
		{
			Target = target;
			Destination = destination;
		}

		public TeleportCommand(Position location, bool macro = false) : base(macro)
		{
			Location = location;
		}

		public TeleportCommand(IEntityTarget target, Position location, bool macro = false) : base(macro)
		{
			Target = target;
			Location = location;
		}

		public TeleportCommand(IEntityTarget target, Position location, Rotation rotation, bool macro = false) : base(macro)
		{
			Target = target;
			Location = location;
			Rotation = rotation;
		}

		public TeleportCommand(IEntityTarget target, Position location, Position facingLocation, bool macro = false) : base(macro)
		{
			Target = target;
			Location = location;
			FacingLocation = facingLocation;
		}

		public TeleportCommand(IEntityTarget target, Position location, IEntityTarget facingEntity, bool eyes = false, bool macro = false) : base(macro)
		{
			Target = target;
			Location = location;
			FacingEntity = facingEntity;
			Eyes = eyes;
		}

		protected override string PreBuild()
		{
			if (Destination is not null && Target is null && Location is null && Rotation is null && FacingLocation is null && FacingEntity is null)
			{
				return $"tp {Destination.Get()}";
			}

			if (Target is not null && Destination is not null && FacingLocation is null && FacingEntity is null)
			{
				return $"tp {Target.Get()} {Destination.Get()}";
			}

			if (Destination is null && Target is null && Location is not null && Rotation is null && FacingLocation is null && FacingEntity is null)
			{
				return $"tp {Location}";
			}

			if (Target is not null && Location is not null && Rotation is null && FacingLocation is null && FacingEntity is null)
			{
				return $"tp {Target.Get()} {Location}";
			}

			if (Target is not null && Location is not null && Rotation is not null && FacingLocation is null && FacingEntity is null)
			{
				return $"tp {Target.Get()} {Location} {Rotation}";
			}

			if (Target is not null && Location is not null && Rotation is null && FacingLocation is not null && FacingEntity is null)
			{
				return $"tp {Target.Get()} {Location} facing {FacingLocation}";
			}

			if (Target is not null && Location is not null && Rotation is null && FacingLocation is null && FacingEntity is not null)
			{
				return $"tp {Target.Get()} {Location} facing entity {FacingEntity.Get()} {(Eyes ? "eyes" : "feet")}";
			}

			throw new ArgumentException("Malformed TeleportCommand");
		}
	}
}
