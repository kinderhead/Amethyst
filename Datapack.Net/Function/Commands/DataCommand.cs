using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public static class DataCommand
    {
        public class Get : Command
        {
            public readonly Position? TargetPos;
            public readonly IEntityTarget? EntityTarget;
            public readonly Storage? StorageTarget;

            public readonly string? Path;
            public readonly double? Scale;

            public Get(Position pos, string? path = null, double? scale = null, bool macro = false) : base(macro)
            {
                TargetPos = pos;
                Path = path;
                Scale = scale;
            }

            public Get(IEntityTarget target, string? path = null, double? scale = null, bool macro = false) : base(macro)
            {
                EntityTarget = target.RequireOne();
                Path = path;
                Scale = scale;
            }

            public Get(Storage target, string? path = null, double? scale = null, bool macro = false) : base(macro)
            {
                StorageTarget = target;
                Path = path;
                Scale = scale;
            }

            protected override string PreBuild()
            {
                var target = "";
                if (TargetPos != null) target = "block " + TargetPos.ToString();
                if (EntityTarget != null) target = "entity " + EntityTarget.Get();
                if (StorageTarget != null) target = "storage " + StorageTarget.ToString();

                if (Scale != null && Path == null)
                {
                    throw new ArgumentException("Invalid syntax. Path cannot be null here.");
                }

                return $"data get {target} {Path} {Scale}";
            }
        }

        public class Merge : Command
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

        public class Modify : Command
        {
            public readonly Position? TargetPos;
            public readonly IEntityTarget? EntityTarget;
            public readonly Storage? StorageTarget;
            public readonly string TargetPath;

            public Position? SourcePos;
            public IEntityTarget? EntitySource;
            public Storage? StorageSource;
            public string? SourcePath;

            public int? Start;
            public int? End;
            public string? Val;

            public string Type;
            public string Modifier;

            public Modify(Position target, string targetPath, bool macro = false) : base(macro)
            {
                TargetPos = target;
                TargetPath = targetPath;
            }

            public Modify(IEntityTarget target, string targetPath, bool macro = false) : base(macro)
            {
                EntityTarget = target.RequireOne();
                TargetPath = targetPath;
            }

            public Modify(Storage target, string targetPath, bool macro = false) : base(macro)
            {
                StorageTarget = target;
                TargetPath = targetPath;
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

            public Modify Merge()
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

            public Modify From(Position source, string? sourcePath = null)
            {
                Modifier = "from";
                SourcePos = source;
                SourcePath = sourcePath;
                return this;
            }

            public Modify From(IEntityTarget source, string? sourcePath = null)
            {
                Modifier = "from";
                EntitySource = source.RequireOne();
                SourcePath = sourcePath;
                return this;
            }

            public Modify From(Storage source, string? sourcePath = null)
            {
                Modifier = "from";
                StorageSource = source;
                SourcePath = sourcePath;
                return this;
            }

            public Modify String(Position source, string? sourcePath = null, int? start = null, int? end = null)
            {
                Modifier = "string";
                SourcePos = source;
                SourcePath = sourcePath;
                Start = start;
                End = end;
                return this;
            }

            public Modify String(IEntityTarget source, string? sourcePath = null, int? start = null, int? end = null)
            {
                Modifier = "string";
                EntitySource = source.RequireOne();
                SourcePath = sourcePath;
                Start = start;
                End = end;
                return this;
            }

            public Modify String(Storage source, string? sourcePath = null, int? start = null, int? end = null)
            {
                Modifier = "string";
                StorageSource = source;
                SourcePath = sourcePath;
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

                if (TargetPos != null) builder.Append($"block {TargetPos}");
                else if (EntityTarget != null) builder.Append($"entity {EntityTarget.Get()}");
                else if (StorageTarget != null) builder.Append($"storage {StorageTarget}");
                else throw new ArgumentException("Invalid data modify command");

                builder.Append($" {TargetPath} {Type} {Modifier} ");

                if (Val != null)
                {
                    builder.Append(Val);
                    return builder.ToString();
                }

                if (SourcePos != null) builder.Append($"block {SourcePos}");
                else if (EntitySource != null) builder.Append($"entity {EntitySource.Get()}");
                else if (StorageSource != null) builder.Append($"storage {StorageSource}");
                else throw new ArgumentException("Invalid data modify command");

                builder.Append($" {SourcePath ?? ""} {Start?.ToString() ?? ""} {End?.ToString() ?? ""}");

                return builder.ToString();
            }
        }

        public class Remove : Command
        {
            public readonly Position? TargetPos;
            public readonly IEntityTarget? EntityTarget;
            public readonly Storage? StorageTarget;
            public readonly string TargetPath;

            public Remove(Position target, string targetPath, bool macro = false) : base(macro)
            {
                TargetPos = target;
                TargetPath = targetPath;
            }

            public Remove(IEntityTarget target, string targetPath, bool macro = false) : base(macro)
            {
                EntityTarget = target.RequireOne();
                TargetPath = targetPath;
            }

            public Remove(Storage target, string targetPath, bool macro = false) : base(macro)
            {
                StorageTarget = target;
                TargetPath = targetPath;
            }

            protected override string PreBuild()
            {
                var builder = new StringBuilder($"data remove ");

                if (TargetPos != null) builder.Append($"block {TargetPos}");
                else if (EntityTarget != null) builder.Append($"entity {EntityTarget.Get()}");
                else if (StorageTarget != null) builder.Append($"storage {StorageTarget}");
                else throw new ArgumentException("Invalid data remove command");

                builder.Append($" {TargetPath}");

                return builder.ToString();
            }
        }
    }
}
