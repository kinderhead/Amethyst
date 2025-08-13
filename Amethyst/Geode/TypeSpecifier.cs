using Amethyst.AST;
using Datapack.Net.Data;

namespace Amethyst.Geode
{
	public abstract class TypeSpecifier
	{
		public abstract bool Operable { get; }
		public virtual bool IsList => false;
		public virtual TypeSpecifier? PossibleInnerType => null;

		// TODO: Make this not throw an error
		public virtual NBTType EffectiveType => throw new InvalidOperationException(ToString());

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

		public abstract override string ToString();

		protected abstract bool AreEqual(TypeSpecifier obj);

		public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
	}

	public class VoidTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;

		protected override bool AreEqual(TypeSpecifier obj) => obj is VoidTypeSpecifier;
		public override string ToString() => "void";
	}

	public class VarTypeSpecifier : TypeSpecifier
	{
		public override bool Operable => false;
		protected override bool AreEqual(TypeSpecifier obj) => obj is VarTypeSpecifier;
		public override string ToString() => "var";
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
		public override string ToString() => Enum.GetName(Type)?.ToLower() ?? throw new InvalidOperationException();

		public static PrimitiveTypeSpecifier Int => new(NBTType.Int);
		public static PrimitiveTypeSpecifier Bool => new(NBTType.Boolean);
		public static PrimitiveTypeSpecifier Compound => new(NBTType.Compound);
		public static PrimitiveTypeSpecifier List => new(NBTType.List);
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

		// TODO: properly do this
		public override string ToString() => $"{ReturnType}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";
		public string ToString(string name) => $"{ReturnType} {name}({string.Join(", ", Parameters.Select(p => $"{p.Type} {p.Name}"))})";
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
		public override string ToString() => $"{Inner}[]";
	}
}
