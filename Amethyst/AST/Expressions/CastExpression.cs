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
			else if (Type.Operable || src.Type.Operable)
			{
				var tmp = ctx.AllocTemp(Type);
				ctx.Add(new StoreAsTypeInstruction(Location, tmp, src, (NBTNumberType)Type.EffectiveType));
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
			else if (Type.Operable || src.Type.Operable) val.Store(ctx, src, (NBTNumberType)Type.EffectiveType);
			else throw new InvalidCastError(Location, src.Type, Type);
		}

		private (Value val, Value? post) PreExecute(FunctionContext ctx)
		{
			var val = Expression.Execute(ctx);
			if (Type == val.Type) return (val, val);
			else if (Type is VoidTypeSpecifier || Type is VarTypeSpecifier) throw new InvalidCastError(Location, val.Type, Type);
			else if (val is LiteralValue l)
			{
				if (l.Value is NBTList list && Type is ListTypeSpecifier lType)
				{
					foreach (var i in list)
					{
						if (i.Type != lType.Inner.EffectiveType) return (val, null);
					}

					return (val, new LiteralValue(list, lType));
				}
				else if (l.Value is INBTNumber num && NBTValue.IsNumberType(Type.EffectiveType))
				{
					switch (Type.EffectiveType)
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
			}
			return (val, null);
		}
	}
}
