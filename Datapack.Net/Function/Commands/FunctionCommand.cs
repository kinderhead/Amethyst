using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Datapack.Net.Function.Commands
{
	public class FunctionCommand : Command
	{
		public readonly NamespacedID Function;
		public readonly NBTCompound? NBTArguments;
		public readonly IEntityTarget? EntityArguments;
		public readonly Storage? StorageArguments;
		public readonly Position? BlockArguments;
		public readonly string Path = "";

		public FunctionCommand(NamespacedID func, bool macro = false) : base(macro)
		{
			Function = func;
		}

		public FunctionCommand(NamespacedID func, NBTCompound arguments, bool macro = false) : base(macro)
		{
			Function = func;
			NBTArguments = arguments;
		}

		public FunctionCommand(NamespacedID func, IEntityTarget arguments, string path = "", bool macro = false) : base(macro)
		{
			Function = func;
			Path = path;
			EntityArguments = arguments;
		}

		public FunctionCommand(NamespacedID func, Storage arguments, string path = "", bool macro = false) : base(macro)
		{
			Function = func;
			Path = path;
			StorageArguments = arguments;
		}

		public FunctionCommand(NamespacedID func, Position arguments, string path = "", bool macro = false) : base(macro)
		{
			Function = func;
			Path = path;
			BlockArguments = arguments;
		}

		public FunctionCommand(MCFunction func, bool macro = false) : base(macro)
		{
			Function = func.ID;

			if (!func.Partial && func.Macro)
			{
				throw new InvalidOperationException($"Function {func.ID} has macro arguments, but is not called with them");
			}
		}

		public FunctionCommand(MCFunction func, NBTCompound arguments, bool macro = false) : base(macro)
		{
			Function = func.ID;
			NBTArguments = arguments;

			if (!func.Partial && !func.Macro)
			{
				throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
			}
		}

		public FunctionCommand(MCFunction func, IEntityTarget arguments, string path = "", bool macro = false) : base(macro)
		{
			Function = func.ID;
			Path = path;
			EntityArguments = arguments;

			if (!func.Partial && !func.Macro)
			{
				throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
			}
		}

		public FunctionCommand(MCFunction func, Storage arguments, string path = "", bool macro = false) : base(macro)
		{
			Function = func.ID;
			Path = path;
			StorageArguments = arguments;

			if (!func.Partial && !func.Macro)
			{
				throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
			}
		}

		public FunctionCommand(MCFunction func, Position arguments, string path = "", bool macro = false) : base(macro)
		{
			Function = func.ID;
			Path = path;
			BlockArguments = arguments;

			if (!func.Partial && !func.Macro)
			{
				throw new InvalidOperationException($"Function {func.ID} does not has macro arguments, but is called with them");
			}
		}

		protected override string PreBuild()
		{
			if (NBTArguments != null)
			{
				return $"function {Function} {NBTArguments}";
			}
			else if (EntityArguments != null)
			{
				return $"function {Function} with entity {EntityArguments.Get()} {Path}".Trim();
			}
			else if (StorageArguments != null)
			{
				return $"function {Function} with storage {StorageArguments} {Path}".Trim();
			}
			else if (BlockArguments != null)
			{
				return $"function {Function} with block {BlockArguments} {Path}".Trim();
			}
			else
			{
				return $"function {Function}";
			}
		}
	}
}
