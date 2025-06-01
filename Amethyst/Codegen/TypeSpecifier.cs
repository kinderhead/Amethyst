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
		public abstract NBTType Type { get; }

		public static bool operator==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator!=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);

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
	}

	public class VoidTypeSpecifier : TypeSpecifier
	{
		public override NBTType Type => NBTType.Byte;

		protected override bool AreEqual(TypeSpecifier obj) => obj is VoidTypeSpecifier;

		protected override string AsString() => "void";
	}

	public class PrimitiveTypeSpecifier(NBTType type) : TypeSpecifier
	{
		public override NBTType Type => type;

		protected override bool AreEqual(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		protected override string AsString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();
	}

	public class DynamicFunctionTypeSpecifier(TypeSpecifier returnType) : TypeSpecifier
	{
		public readonly TypeSpecifier ReturnType = returnType;

		public override NBTType Type => NBTType.String;

		protected override bool AreEqual(TypeSpecifier obj) => obj is DynamicFunctionTypeSpecifier f && f.ReturnType == ReturnType;
		protected override string AsString() => $"() => {ReturnType}";
	}

	public class FunctionTypeSpecifier(TypeSpecifier returnType) : DynamicFunctionTypeSpecifier(returnType)
	{
		public override NBTType Type => NBTType.String;

		protected override bool AreEqual(TypeSpecifier obj) => base.AreEqual(obj) && obj is FunctionTypeSpecifier;
		protected override string AsString() => $"() => {ReturnType}";
	}
}
