using Datapack.Net.Data;
using Datapack.Net.Data._1_20_4;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;

namespace Datapack.Net.CubeLib
{
	public class Entity(ScoreRef id) : IRuntimeArgument
	{
		internal static readonly Stack<Entity?> AsStack = [];
		internal static readonly Stack<Entity?> AtStack = [];

		private readonly Dictionary<string, ScoreRef> Uniques = [];

		public readonly ScoreRef ID = id;

		public ScoreRef GetAsArg() => ID;

		public ScoreRef Health
		{
			get
			{
				var ret = Unique("health");
				Project.ActiveProject.AddCommand(As(ret.Store()).Run(new DataCommand.Get(TargetSelector.Self, "Health")));
				return ret.AsReadonly();
			}

			set => As(() =>
								{
									var diff = Health - value;
									Project.ActiveProject.Std.Damage([new("value", diff)]);
								}, false);
		}

		public EntityProperty<T> Get<T>(string path) where T : NBTValue => new() { Entity = this, Path = path };
		public T GetAs<T>(string path) where T : EntityProperty, new() => new() { Entity = this, Path = path };

		public void As(Action func, bool at = true, bool force = false)
		{
			if (!force && AsStack.TryPeek(out var cur) && cur == this && (!at || (AtStack.TryPeek(out var atcur) && atcur == this)))
			{
				func();
				return;
			}

			var proj = Project.ActiveProject;
			var cmd = As(new Execute(), force).Run(proj.Lambda(() =>
			{
				AsStack.Push(this);
				if (at)
				{
					AtStack.Push(this);
				}

				func();

				_ = AsStack.Pop();
				if (at)
				{
					_ = AtStack.Pop();
				}
			}));
			if (at)
			{
				_ = cmd.At(TargetSelector.Self);
			}

			proj.AddCommand(cmd);
		}

		public void Teleport(Position pos) => Project.ActiveProject.AddCommand(As(new Execute().Run(new TeleportCommand(pos))));

		public void Teleport(Entity dest) => Project.ActiveProject.AddCommand(As(dest.As(new Execute().Run(new TeleportCommand(new Position(new(0, CoordType.Relative), new(0, CoordType.Relative), new(0, CoordType.Relative))))).At(TargetSelector.Self), true));

		public void Kill() => Project.ActiveProject.AddCommand(As(new Execute().Run(new KillCommand(TargetSelector.Self))));

		public void CopyTo(IPointer<NBTCompound> loc) => As(() => Project.ActiveProject.Std.EntityNBTToPointer(loc.StandardMacros()), false);
		public void CopyTo<T>(IPointer<T> loc, string path) where T : NBTValue => As(() => Project.ActiveProject.Std.EntityNBTToPointerPath(loc.StandardMacros([new("epath", path)])), false);

		public void SetNBT(string path, NBTValue value)
		{
			var cmd = new DataCommand.Modify(TargetSelector.Self, path).Set().Value(value.Build());
			if (NeedsPlayerCheck())
			{
				As(() =>
				{
					PlayerCheck();
					Project.ActiveProject.AddCommand(cmd);
				}, false);
			}
			else
			{
				Project.ActiveProject.AddCommand(As(new Execute()).Run(cmd));
			}
		}

		public void SetNBT<T>(string path, IPointer<T> value) where T : NBTValue => As(() =>
																							 {
																								 PlayerCheck();
																								 Project.ActiveProject.Std.EntityWrite([new("path", path), new("value", value)]);
																							 }, false);

		public void RemoveNBT(string path)
		{
			var cmd = new DataCommand.Remove(TargetSelector.Self, path);
			if (NeedsPlayerCheck())
			{
				As(() =>
				{
					PlayerCheck();
					Project.ActiveProject.AddCommand(cmd);
				});
			}
			else
			{
				Project.ActiveProject.AddCommand(As(new Execute()).Run(cmd));
			}
		}

		public virtual bool NeedsPlayerCheck() => Project.Settings.EntityCheckPlayer;

		public virtual void PlayerCheck()
		{
			if (!NeedsPlayerCheck())
			{
				return;
			}

			Project.ActiveProject.If(Is(new TargetSelector(TargetType.s, type: Entities.Player)), new TellrawCommand(new TargetSelector(TargetType.a), new FormattedText().Entity(TargetSelector.Self).Text(" is a player and cannot be edited")));
		}

		/// <summary>
		/// Sets the execute's target to the entity. If ran inside <see cref="As(Action, bool)"/>
		/// then the current entity will be remembered and <c>@s</c> will be used.
		/// Setting <c>force</c> to true will always recalculate the current entity.
		/// </summary>
		/// <param name="cmd">Execute command</param>
		/// <param name="force">Force recalculation</param>
		/// <returns></returns>
		public Execute As(Execute cmd, bool force = false)
		{
			if (force || AsStack.Count == 0 || (AsStack.TryPeek(out var cur) && cur != this))
			{
				return cmd.As(new TargetSelector(TargetType.e)).If.Score(TargetSelector.Self, Project.EntityIDScore, Comparison.Equal, ID.Target, ID.Score);
			}

			return cmd;
		}

		public bool IsCurrentTarget() => AsStack.TryPeek(out var cur) && cur == this;

		public EntityComparison Is(TargetSelector sel) => new(this, sel);
		public EntityExists Exists() => new(this);

		private ScoreRef Unique(string key)
		{
			if (Uniques.TryGetValue(key, out var score))
			{
				return score;
			}

			score = Project.ActiveProject.NewUnique();
			Uniques[key] = score;
			return score;
		}
	}
}
