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

		public virtual NBTType EffectiveType => throw new InvalidOperationException(ToString());

		protected abstract bool AreEqual(TypeSpecifier obj);
		public override bool Equals(object? obj)
		{
			if (obj is null || obj?.GetType() != GetType()) return false;
			return AreEqual(obj as TypeSpecifier ?? throw new InvalidOperationException());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString() => AsString();
		protected abstract string AsString();

		public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
	}

	public class VoidTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;

        protected override bool AreEqual(TypeSpecifier obj) => obj is VoidTypeSpecifier;
		protected override string AsString() => "void";
	}

	public class VarTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		protected override bool AreEqual(TypeSpecifier obj) => obj is VarTypeSpecifier;
		protected override string AsString() => "var";
	}

	public class PrimitiveTypeSpecifier(NBTType type) : TypeSpecifier
	{
		public override bool Operable => NBTValue.IsOperableType(Type);
        public override bool IsArray => Type == NBTType.List || Type == NBTType.IntArray || Type == NBTType.LongArray || Type == NBTType.ByteArray;
		public readonly NBTType Type = type;
		public override NBTType EffectiveType => Type;

		protected override bool AreEqual(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		protected override string AsString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();
	}

	public class FunctionTypeSpecifier(TypeSpecifier returnType, IEnumerable<TypeSpecifier> paramters) : TypeSpecifier
	{
		public readonly TypeSpecifier ReturnType = returnType;
		public readonly TypeSpecifier[] Parameters = [.. paramters];
		public override bool Operable => false;

        protected override bool AreEqual(TypeSpecifier obj) => obj is FunctionTypeSpecifier f && f.ReturnType == ReturnType;
		protected override string AsString() => $"() => {ReturnType}";
	}

	public class DynamicFunctionTypeSpecifier(TypeSpecifier returnType) : FunctionTypeSpecifier(returnType, [])
	{
		protected override bool AreEqual(TypeSpecifier obj) => base.AreEqual(obj) && obj is DynamicFunctionTypeSpecifier;
		protected override string AsString() => $"() => {ReturnType}";
	}
}
