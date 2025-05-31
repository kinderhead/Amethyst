using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public abstract class Expression(LocationRange loc) : Node(loc)
	{
		public Value Execute(FunctionContext ctx)
		{
			ctx.PushLocator(this);
			try
			{
				return _Execute(ctx);
			}
			catch (Exception) // Saw somewhere that it might be necessary to explicitly catch all errors so finally is run
			{
				throw;
			}
			finally
			{
				ctx.PopLocator();
			}
		}

		// Cast is here so location is populated
		public Value Cast(FunctionContext ctx, TypeSpecifier type)
		{
			var val = Execute(ctx);
			if (type == val.Type) return val;
			else if (
				type is VoidTypeSpecifier ||
				(NBTValue.IsNumberType(val.Type.Type) != NBTValue.IsNumberType(type.Type))
			) throw new InvalidCastError(Location, val.Type, type);
			else if (val is LiteralValue l)
			{
				if (l.Value is not INBTNumber num || !NBTValue.IsNumberType(type.Type)) throw new InvalidCastError(Location, val.Type, type);
				switch (type.Type)
				{
					case NBTType.Boolean:
						return new LiteralValue(new NBTBool((bool)num.RawValue));
					case NBTType.Byte:
						return new LiteralValue(new NBTByte((byte)num.RawValue));
					case NBTType.Short:
						return new LiteralValue(new NBTShort((short)num.RawValue));
					case NBTType.Int:
						return new LiteralValue(new NBTInt((int)num.RawValue));
					case NBTType.Long:
						return new LiteralValue(new NBTLong(Convert.ToInt64(num.RawValue)));
					case NBTType.Float:
						return new LiteralValue(new NBTFloat(Convert.ToSingle(num.RawValue)));
					case NBTType.Double:
						return new LiteralValue(new NBTDouble(Convert.ToDouble(num.RawValue)));
					default:
						break;
				}
			}
			else if (NBTValue.IsOperableType(type.Type) || NBTValue.IsOperableType(val.Type.Type))
			{
				var tmp = ctx.AllocTemp(type);
				ctx.Add(new StoreAsTypeInstruction(Location, tmp, val, (NBTNumberType)type.Type));
				return tmp;
			}

			throw new InvalidCastError(Location, val.Type, type);
		}

		protected abstract Value _Execute(FunctionContext ctx);
	}

	public class LiteralExpression(LocationRange loc, NBTValue val) : Expression(loc)
	{
		public readonly NBTValue Value = val;

		protected override Value _Execute(FunctionContext ctx) => new LiteralValue(Value);
	}

	public class VariableExpression(LocationRange loc, string name) : Expression(loc)
	{
		public readonly string Name = name;

		protected override Value _Execute(FunctionContext ctx) => ctx.GetVariable(Name);
	}
}
