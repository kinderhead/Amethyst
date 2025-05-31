using Amethyst.AST;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen
{
	public abstract class Value
	{
		public abstract TypeSpecifier Type { get; }

		public abstract DataCommand.Modify WriteTo(DataCommand.Modify src);
		public abstract Command Get();
	}

	public class LiteralValue(NBTValue val) : Value
	{
		public readonly NBTValue Value = val;
		public override TypeSpecifier Type => new PrimitiveTypeSpecifier(Value.Type);
		public override Command Get() => throw new InvalidOperationException(); // Maybe implement this
		public override DataCommand.Modify WriteTo(DataCommand.Modify src) => src.Value(Value.ToString());
	}

	public abstract class MutableValue : Value
	{
		public abstract void Store(FunctionContext ctx, Value val);
	}

	public class StorageValue(Storage storage, string path, TypeSpecifier type) : MutableValue
	{
		public readonly Storage Storage = storage;
		public readonly string Path = path;
		public override TypeSpecifier Type => type;

		public override Command Get() => new DataCommand.Get(Storage, Path);

		public override void Store(FunctionContext ctx, Value val)
		{
			ctx.Add(new StoreInstruction(ctx.CurrentLocator.Location, this, val));
		}

		public override DataCommand.Modify WriteTo(DataCommand.Modify src) => src.From(Storage, Path);
	}
}
