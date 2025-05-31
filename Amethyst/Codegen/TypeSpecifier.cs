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
	}

	public class VoidTypeSpecifier : TypeSpecifier
	{
		public override NBTType Type => NBTType.Byte;

		protected override bool AreEqual(TypeSpecifier obj) => obj is VoidTypeSpecifier;

		public override string ToString() => "void";
	}

	public class PrimitiveTypeSpecifier(NBTType type) : TypeSpecifier
	{
		public override NBTType Type => type;

		protected override bool AreEqual(TypeSpecifier obj) => obj is PrimitiveTypeSpecifier p && p.Type == Type;
		public override string ToString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();
	}

	public class FunctionTypeSpecifier(TypeSpecifier returnType) : TypeSpecifier
	{
		public readonly TypeSpecifier ReturnType = returnType;

		public override NBTType Type => NBTType.String;

		protected override bool AreEqual(TypeSpecifier obj) => obj is FunctionTypeSpecifier f && f.ReturnType == ReturnType;
	}
}
