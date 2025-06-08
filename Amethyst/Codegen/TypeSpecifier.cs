using Amethyst.AST;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Codegen
{
	public abstract class TypeSpecifier
	{
		public abstract bool Operable { get; }
		public virtual bool IsArray => false;

		// TODO: Make this not throw an error
		public virtual NBTType EffectiveType => throw new InvalidOperationException(ToString());

		public void Cast(FunctionContext ctx, Value src, ReturnOrStore dest)
		{
			if (dest.Type == this) dest.Store(src);
			else if (src.AsConstant() is NBTValue nbt && nbt is INBTNumber num && NBTValue.IsNumberType(dest.Type.EffectiveType))
			{
				switch (dest.Type.EffectiveType)
				{
					case NBTType.Boolean:
						dest.Store(new LiteralValue(new NBTBool(Convert.ToBoolean(num.RawValue)), dest.Type));
						break;
					case NBTType.Byte:
						dest.Store(new LiteralValue(new NBTByte(Convert.ToByte(num.RawValue)), dest.Type));
						break;
					case NBTType.Short:
						dest.Store(new LiteralValue(new NBTShort(Convert.ToInt16(num.RawValue)), dest.Type));
						break;
					case NBTType.Int:
						dest.Store(new LiteralValue(new NBTInt(Convert.ToInt32(num.RawValue)), dest.Type));
						break;
					case NBTType.Long:
						dest.Store(new LiteralValue(new NBTLong(Convert.ToInt64(num.RawValue)), dest.Type));
						break;
					case NBTType.Float:
						dest.Store(new LiteralValue(new NBTFloat(Convert.ToSingle(num.RawValue)), dest.Type));
						break;
					case NBTType.Double:
						dest.Store(new LiteralValue(new NBTDouble(Convert.ToDouble(num.RawValue)), dest.Type));
						break;
					default:
						break;
				}
			}
			else if (!_Cast(ctx, src, dest)) throw new InvalidCastError(ctx.CurrentLocator.Location, src.Type, dest.Type);
		}

		public override bool Equals(object? obj)
		{
			if (obj is null || obj?.GetType() != GetType()) return false;
			return AreEqual(obj as TypeSpecifier ?? throw new InvalidOperationException());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString() => _ToString();

		protected abstract bool AreEqual(TypeSpecifier obj);
		protected abstract string _ToString();

		protected virtual bool _Cast(FunctionContext ctx, Value src, ReturnOrStore dest) => false;

		public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
	}

	public class VoidTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;

		protected override bool AreEqual(TypeSpecifier obj) => obj is VoidTypeSpecifier;
		protected override string _ToString() => "void";
	}

	public class VarTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		protected override bool AreEqual(TypeSpecifier obj) => obj is VarTypeSpecifier;
		protected override string _ToString() => "var";
	}

	public class PrimitiveTypeSpecifier(NBTType type) : TypeSpecifier
	{
		public override bool Operable => NBTValue.IsOperableType(Type);
		public override bool IsArray => Type == NBTType.List || Type == NBTType.IntArray || Type == NBTType.LongArray || Type == NBTType.ByteArray;
		public readonly NBTType Type = type;
		public override NBTType EffectiveType => Type;

		protected override bool AreEqual(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		protected override string _ToString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();

		protected override bool _Cast(FunctionContext ctx, Value src, ReturnOrStore dest)
		{
			if (NBTValue.IsNumberType(Type) && dest.Type.EffectiveType == NBTType.Boolean)
			{
				var newDest = dest.RequireStorage();
				var args = new Dictionary<string, Value>
				{
					["dest_storage"] = new LiteralValue(newDest.Storage.ToString()),
					["dest_path"] = new LiteralValue(newDest.Path),
					["value"] = src
				};
				ctx.Add(new CallInstruction(ctx.CurrentLocator.Location, "amethyst:core/bool", [], args));
				return true;
			}
			else if (NBTValue.IsNumberType(dest.Type.EffectiveType) && (NBTValue.IsOperableType(dest.Type.EffectiveType) || NBTValue.IsOperableType(EffectiveType)))
			{
				dest.Store(src, (NBTNumberType)dest.Type.EffectiveType);
				return true;
			}
			else if (src.AsConstant() is NBTList list && dest.Type is ListTypeSpecifier lType)
			{
				foreach (var i in list)
				{
					if (i.Type != lType.Inner.EffectiveType) return false;
				}

				dest.Store(new LiteralValue(list, lType));
				return true;
			}

			return false;
		}
	}

	public class FunctionTypeSpecifier(TypeSpecifier returnType, IEnumerable<Parameter> paramters) : TypeSpecifier
	{
		public readonly TypeSpecifier ReturnType = returnType;
		public readonly Parameter[] Parameters = [.. paramters];
		public override bool Operable => false;

		protected override bool AreEqual(TypeSpecifier obj) => obj is FunctionTypeSpecifier f
			&& f.ReturnType == ReturnType
			&& Parameters.Length == f.Parameters.Length
			&& Parameters.Zip(f.Parameters).All(i => i.First == i.Second);
		protected override string _ToString() => $"() => {ReturnType}";
	}

	public class DynamicFunctionTypeSpecifier(TypeSpecifier returnType) : FunctionTypeSpecifier(returnType, [])
	{
		protected override bool AreEqual(TypeSpecifier obj) => base.AreEqual(obj) && obj is DynamicFunctionTypeSpecifier;
		protected override string _ToString() => $"() => {ReturnType}";
	}

	public class ListTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
	{
		public readonly TypeSpecifier Inner = inner;
		public override bool Operable => false;
		public override NBTType EffectiveType => NBTType.List;
		protected override bool AreEqual(TypeSpecifier obj) => obj is ListTypeSpecifier arr && arr.Inner == Inner;
		protected override string _ToString() => $"{Inner}[]";
	}
}
