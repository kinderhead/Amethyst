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
	public class CastExpression(LocationRange loc, TypeSpecifier type, Expression expr) : Expression(loc)
	{
		public readonly TypeSpecifier Type = type;
		public readonly Expression Expression = expr;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => Type;

		protected override Value _Execute(FunctionContext ctx)
		{
			if (Expression.ComputeType(ctx) == Type) return Expression.Execute(ctx);

			(Value src, Value? post) = PreExecute(ctx);

			if (post is not null) return post;
			else if (NBTValue.IsOperableType(Type.Type) || NBTValue.IsOperableType(src.Type.Type))
			{
				var tmp = ctx.AllocTemp(Type);
				ctx.Add(new StoreAsTypeInstruction(Location, tmp, src, (NBTNumberType)Type.Type));
				return tmp;
			}

			throw new InvalidCastError(Location, src.Type, Type);
		}

		protected override void _Store(FunctionContext ctx, MutableValue val)
		{
			if (Expression.ComputeType(ctx) == Type)
			{
				Expression.Store(ctx, val);
				return;
			}

			(Value src, Value? post) = PreExecute(ctx);

			if (post is not null) val.Store(ctx, post);
			else if (NBTValue.IsOperableType(Type.Type) || NBTValue.IsOperableType(src.Type.Type)) val.Store(ctx, src, (NBTNumberType)Type.Type);
			else throw new InvalidCastError(Location, src.Type, Type);
		}

		private (Value val, Value? post) PreExecute(FunctionContext ctx)
		{
			var val = Expression.Execute(ctx);
			if (Type == val.Type) return (val, val);
			else if (
				Type is VoidTypeSpecifier ||
				(NBTValue.IsNumberType(val.Type.Type) != NBTValue.IsNumberType(Type.Type))
			) throw new InvalidCastError(Location, val.Type, Type);
			else if (val is LiteralValue l)
			{
				if (l.Value is not INBTNumber num || !NBTValue.IsNumberType(Type.Type)) throw new InvalidCastError(Location, val.Type, Type);
				switch (Type.Type)
				{
					case NBTType.Boolean:
						return (val, new LiteralValue(new NBTBool(Convert.ToBoolean(num.RawValue))));
					case NBTType.Byte:
						return (val, new LiteralValue(new NBTByte(Convert.ToByte(num.RawValue))));
					case NBTType.Short:
						return (val, new LiteralValue(new NBTShort(Convert.ToInt16(num.RawValue))));
					case NBTType.Int:
						return (val, new LiteralValue(new NBTInt(Convert.ToInt32(num.RawValue))));
					case NBTType.Long:
						return (val, new LiteralValue(new NBTLong(Convert.ToInt64(num.RawValue))));
					case NBTType.Float:
						return (val, new LiteralValue(new NBTFloat(Convert.ToSingle(num.RawValue))));
					case NBTType.Double:
						return (val, new LiteralValue(new NBTDouble(Convert.ToDouble(num.RawValue))));
					default:
						break;
				}
			}
			return (val, null);
		}
	}
}
