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
    }
}
