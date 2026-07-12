using Datapack.Net.Data;
using System.Text;

namespace Datapack.Net.Function.Commands
{
	public abstract class DataCommand(bool macro) : Command(macro)
	{
		public class Get(IDataTarget target, double? scale = null, bool macro = false) : DataCommand(macro)
		{
			public readonly double? Scale = scale;
			public readonly IDataTarget Target = target;

			public Get(Position pos, string? path = null, double? scale = null, bool macro = false) : this(
				new BlockDataTarget(pos, path), scale, macro)
			{
			}

			public Get(IEntityTarget target, string? path = null, double? scale = null, bool macro = false) : this(
				new EntityDataTarget(target, path), scale, macro)
			{
			}

			public Get(Storage target, string? path = null, double? scale = null, bool macro = false) : this(
				new StorageTarget(target, path), scale, macro)
			{
			}

			protected override string PreBuild()
			{
				if (Scale is not null && Target.Path is null)
				{
					throw new ArgumentException("Invalid syntax. Path cannot be null here.");
				}

				return $"data get {Target.GetTarget()} {Scale}";
			}
		}

		public class Merge : DataCommand
		{
			public readonly IEntityTarget? EntityTarget;

			public readonly NBTCompound NBT;
			public readonly Storage? StorageTarget;
			public readonly Position? TargetPos;

			public Merge(Position pos, NBTCompound nbt, bool macro = false) : base(macro)
			{
				TargetPos = pos;
				NBT = nbt;
			}

			public Merge(IEntityTarget target, NBTCompound nbt, bool macro = false) : base(macro)
			{
				EntityTarget = target.RequireOne();
				NBT = nbt;
			}

			public Merge(Storage target, NBTCompound nbt, bool macro = false) : base(macro)
			{
				StorageTarget = target;
				NBT = nbt;
			}

			protected override string PreBuild()
			{
				var target = "";
				if (TargetPos != null)
				{
					target = "block " + TargetPos;
				}

				if (EntityTarget != null)
				{
					target = "entity " + EntityTarget.Get();
				}

				if (StorageTarget != null)
				{
					target = "storage " + StorageTarget;
				}

				return $"data merge {target} {NBT}";
			}
		}

		public class Modify(IDataTarget target, bool macro = false) : DataCommand(macro)
		{
			public readonly IDataTarget Target = target;
			public int? End;
			public string? Modifier;
			public IDataTarget? Source;

			public int? Start;

			public string? Type;
			public string? Val;

			public Modify(Position target, string targetPath, bool macro = false) : this(
				new BlockDataTarget(target, targetPath), macro)
			{
			}

			public Modify(IEntityTarget target, string targetPath, bool macro = false) : this(
				new EntityDataTarget(target, targetPath), macro)
			{
			}

			public Modify(Storage target, string targetPath, bool macro = false) : this(
				new StorageTarget(target, targetPath), macro)
			{
			}

			public Modify Append()
			{
				Type = "append";
				return this;
			}

			public Modify Insert(int index)
			{
				Type = $"insert {index}";
				return this;
			}

			public new Modify Merge()
			{
				Type = "merge";
				return this;
			}

			public Modify Prepend()
			{
				Type = "prepend";
				return this;
			}

			public Modify Set()
			{
				Type = "set";
				return this;
			}

			public Modify From(IDataTarget target)
			{
				Modifier = "from";
				Source = target;
				return this;
			}

			public Modify From(Position source, string? sourcePath = null)
			{
				Modifier = "from";
				Source = new BlockDataTarget(source, sourcePath);
				return this;
			}

			public Modify From(IEntityTarget source, string? sourcePath = null)
			{
				Modifier = "from";
				Source = new EntityDataTarget(source, sourcePath);
				return this;
			}

			public Modify From(Storage source, string? sourcePath = null)
			{
				Modifier = "from";
				Source = new StorageTarget(source, sourcePath);
				return this;
			}

			public Modify String(Position source, string? sourcePath = null, int? start = null, int? end = null)
			{
				Modifier = "string";
				Source = new BlockDataTarget(source, sourcePath);
				Start = start;
				End = end;
				return this;
			}

			public Modify String(IEntityTarget source, string? sourcePath = null, int? start = null, int? end = null)
			{
				Modifier = "string";
				Source = new EntityDataTarget(source, sourcePath);
				Start = start;
				End = end;
				return this;
			}

			public Modify String(Storage source, string? sourcePath = null, int? start = null, int? end = null)
			{
				Modifier = "string";
				Source = new StorageTarget(source, sourcePath);
				Start = start;
				End = end;
				return this;
			}

			public Modify Value(string value)
			{
				Val = value;
				Modifier = "value";
				return this;
			}

			protected override string PreBuild()
			{
				var builder = new StringBuilder("data modify ");

				builder.Append($"{Target.GetTarget()} {Type} {Modifier} ");

				if (Val != null)
				{
					builder.Append(Val);
					return builder.ToString();
				}

				builder.Append($"{Source?.GetTarget() ?? ""} {Start?.ToString() ?? ""} {End?.ToString() ?? ""}");

				return builder.ToString();
			}
		}

		public class Remove(IDataTarget target, bool macro = false) : DataCommand(macro)
		{
			public readonly IDataTarget Target = target;

			public Remove(Position target, string targetPath, bool macro = false) : this(
				new BlockDataTarget(target, targetPath), macro)
			{
			}

			public Remove(IEntityTarget target, string targetPath, bool macro = false) : this(
				new EntityDataTarget(target, targetPath), macro)
			{
			}

			public Remove(Storage target, string targetPath, bool macro = false) : this(
				new StorageTarget(target, targetPath), macro)
			{
			}

			protected override string PreBuild() => $"data remove {Target.GetTarget()}";
		}
	}
}