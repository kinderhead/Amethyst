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
		public virtual bool IsList => false;
		public virtual TypeSpecifier? PossibleInnerType => null;

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
						dest.Store(new LiteralValue(new NBTByte(Convert.ToSByte(num.RawValue)), dest.Type));
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
			else if (dest.Type is PrimitiveTypeSpecifier t && t.Type == NBTType.Compound) dest.Store(src); // TODO: maybe make fix for if dest might return a score
			else if (!_Cast(ctx, src, dest)) throw new InvalidCastError(ctx.CurrentLocator.Location, src.Type, dest.Type);
		}

		public virtual bool IsAssignableTo(TypeSpecifier other) => this == other;
		public virtual TypeSpecifier? Property(string name) => null;

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
		public override bool IsList => Type == NBTType.List || Type == NBTType.IntArray || Type == NBTType.LongArray || Type == NBTType.ByteArray;
		public readonly NBTType Type = type;
		public override NBTType EffectiveType => Type;

        public override bool IsAssignableTo(TypeSpecifier other) => (other.IsList && Type == NBTType.List) || base.IsAssignableTo(other);
		public override TypeSpecifier? Property(string name) => Type == NBTType.Compound ? new PrimitiveTypeSpecifier(NBTType.Compound) : base.Property(name);

		protected override bool AreEqual(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		protected override string _ToString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();

		protected override bool _Cast(FunctionContext ctx, Value src, ReturnOrStore dest)
		{
			if (NBTValue.IsNumberType(Type) && dest.Type.EffectiveType == NBTType.Boolean)
			{
				var newDest = dest.RequireStorage();
				ctx.Call("amethyst:core/bool", [new LiteralValue(newDest.Storage.ToString()), new LiteralValue(newDest.Path), src]);
				return true;
			}
			else if (NBTValue.IsNumberType(dest.Type.EffectiveType) && (NBTValue.IsOperableType(dest.Type.EffectiveType) || NBTValue.IsOperableType(EffectiveType)))
			{
				dest.Store(src, (NBTNumberType)dest.Type.EffectiveType);
				return true;
			}
			else if (Type == NBTType.List && dest.Type is ListTypeSpecifier)
			{
				dest.Store(src);
				return true;
			}
			else if (Type == NBTType.Compound && dest.Type.EffectiveType == NBTType.Compound)
			{
				dest.Store(src);
				return true;
			}

			return false;
		}
	}

	public class FunctionTypeSpecifier(FunctionModifiers modifiers, TypeSpecifier returnType, IEnumerable<Parameter> paramters) : TypeSpecifier
	{
		public readonly FunctionModifiers Modifiers = modifiers;
		public readonly TypeSpecifier ReturnType = returnType;
		public readonly Parameter[] Parameters = [.. paramters];
		public override bool Operable => false;

		protected override bool AreEqual(TypeSpecifier obj) => obj is FunctionTypeSpecifier f
			&& f.Modifiers == Modifiers
			&& f.ReturnType == ReturnType
			&& Parameters.Length == f.Parameters.Length
			&& Parameters.Zip(f.Parameters).All(i => i.First == i.Second);
		protected override string _ToString() => $"() => {ReturnType}";
	}

	public class DynamicFunctionTypeSpecifier(TypeSpecifier returnType) : FunctionTypeSpecifier(FunctionModifiers.None, returnType, [])
	{
		protected override bool AreEqual(TypeSpecifier obj) => base.AreEqual(obj) && obj is DynamicFunctionTypeSpecifier;
	}

	public class ListTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
	{
		public readonly TypeSpecifier Inner = inner;
		public override bool Operable => false;
		public override NBTType EffectiveType => NBTType.List;
		public override TypeSpecifier? PossibleInnerType => Inner;
        public override bool IsList => true;
		// public override bool IsAssignableTo(TypeSpecifier other) => other.EffectiveType == NBTType.List || base.IsAssignableTo(other);
		protected override bool AreEqual(TypeSpecifier obj) => obj is ListTypeSpecifier arr && arr.Inner == Inner;
		protected override string _ToString() => $"{Inner}[]";
	}
}
