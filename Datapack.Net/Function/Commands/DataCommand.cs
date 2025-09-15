using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public abstract class DataCommand(bool macro) : Command(macro)
    {
        public class Get(IDataTarget target, double? scale = null, bool macro = false) : DataCommand(macro)
		{
            public readonly IDataTarget Target = target;

            public readonly double? Scale = scale;

            public Get(Position pos, string? path = null, double? scale = null, bool macro = false) : this(new BlockDataTarget(pos, path), scale, macro) { }
            public Get(IEntityTarget target, string? path = null, double? scale = null, bool macro = false) : this(new EntityDataTarget(target, path), scale, macro) { }
            public Get(Storage target, string? path = null, double? scale = null, bool macro = false) : this(new StorageTarget(target, path), scale, macro) { }

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
            public readonly Position? TargetPos;
            public readonly IEntityTarget? EntityTarget;
            public readonly Storage? StorageTarget;

            public readonly NBTCompound NBT;

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
                if (TargetPos != null) target = "block " + TargetPos.ToString();
                if (EntityTarget != null) target = "entity " + EntityTarget.Get();
                if (StorageTarget != null) target = "storage " + StorageTarget.ToString();

                return $"data merge {target} {NBT}";
            }
        }

        public class Modify(IDataTarget target, bool macro = false) : DataCommand(macro)
		{
            public readonly IDataTarget Target = target;
            public IDataTarget Source;

            public int? Start;
            public int? End;
            public string? Val;

            public string Type;
            public string Modifier;

            public Modify(Position target, string targetPath, bool macro = false) : this(new BlockDataTarget(target, targetPath), macro) { }
            public Modify(IEntityTarget target, string targetPath, bool macro = false) : this(new EntityDataTarget(target, targetPath), macro) { }
            public Modify(Storage target, string targetPath, bool macro = false) : this(new StorageTarget(target, targetPath), macro) { }

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

            new public Modify Merge()
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
                var builder = new StringBuilder($"data modify ");

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
            public readonly string TargetPath;

            public Remove(Position target, string targetPath, bool macro = false) : this(new BlockDataTarget(target, targetPath), macro) { }
            public Remove(IEntityTarget target, string targetPath, bool macro = false) : this(new EntityDataTarget(target, targetPath), macro) { }
            public Remove(Storage target, string targetPath, bool macro = false) : this(new StorageTarget(target, targetPath), macro) { }

            protected override string PreBuild()
            {
                return $"data remove {Target.GetTarget()}";
            }
        }
    }
}
