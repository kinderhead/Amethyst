using Datapack.Net.Data;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Function.Commands
{
    public class FunctionCommand : Command
    {
        public readonly MCFunction Function;
        public readonly NBTCompound? NBTArguments;
        public readonly IEntityTarget? EntityArguments;
        public readonly Storage? StorageArguments;
        public readonly Position? BlockArguments;
        public readonly string Path = "";

        public FunctionCommand(MCFunction func, bool macro = false) : base(macro)
        {
            Function = func;

            if (Function.Macro) throw new InvalidOperationException($"Function {func.ID} has macro arguments, but is not called with them");
        }

        public FunctionCommand(MCFunction func, NBTCompound arguments, bool macro = false) : base(macro)
        {
            Function = func;
            NBTArguments = arguments;

            if (!Function.Macro) throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
        }

        public FunctionCommand(MCFunction func, IEntityTarget arguments, string path = "", bool macro = false) : base(macro)
        {
            Function = func;
            Path = path;
            EntityArguments = arguments;

            if (!Function.Macro) throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
        }

        public FunctionCommand(MCFunction func, Storage arguments, string path = "", bool macro = false) : base(macro)
        {
            Function = func;
            Path = path;
            StorageArguments = arguments;

            if (!Function.Macro) throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
        }

        public FunctionCommand(MCFunction func, Position arguments, string path = "", bool macro = false) : base(macro)
        {
            Function = func;
            Path = path;
            BlockArguments = arguments;

            if (!Function.Macro) throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
        }

        protected override string PreBuild()
        {
            if (NBTArguments != null)
            {
                return $"function {Function.ID} {NBTArguments}";
            }
            else if (EntityArguments != null)
            {
                return $"function {Function.ID} with entity {EntityArguments.Get()} {Path}".Trim();
            }
            else if (StorageArguments != null)
            {
                return $"function {Function.ID} with storage {StorageArguments} {Path}".Trim();
            }
            else if (BlockArguments != null)
            {
                return $"function {Function.ID} with block {BlockArguments} {Path}".Trim();
            }
            else
            {
                return $"function {Function.ID}";
            }
        }
    }
}
