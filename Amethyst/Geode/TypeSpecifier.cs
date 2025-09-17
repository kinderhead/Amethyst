using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode
{
	public abstract class TypeSpecifier : ICloneable
	{
		public abstract bool Operable { get; }
		public abstract LiteralValue DefaultValue { get; }
		public virtual bool IsList => false;
		public virtual IEnumerable<TypeSpecifier> Subtypes => [];
		public virtual string BasePath => "minecraft";
		public virtual TypeSpecifier BaseClass => PrimitiveTypeSpecifier.Compound;

		// TODO: Make this not throw an error
		public virtual NBTType EffectiveType => throw new InvalidOperationException(ToString());
		public bool ShouldStoreInScore => EffectiveType == NBTType.Int || EffectiveType == NBTType.Boolean;

		public NBTNumberType? EffectiveNumberType => Enum.IsDefined((NBTNumberType)EffectiveType) ? (NBTNumberType)EffectiveType : null;

		//public virtual bool IsAssignableTo(TypeSpecifier other) => this == other;
		public virtual TypeSpecifier? Property(string name) => null;

		public virtual string MacroGuardStart => "";
		public virtual string MacroGuardEnd => "";

		public override bool Equals(object? obj)
		{
			if (obj is not TypeSpecifier other) return false;
			else return GetEquatableType().AreEqual(other.GetEquatableType());
		}

		public virtual bool Implements(TypeSpecifier other)
		{
			if (this == other) return true;
			else if (this == BaseClass) return false;
			else return BaseClass.Implements(other);
		}

		public virtual bool ConstraintSatisfiedBy(TypeSpecifier other)
		{
			return other.GetType() == GetType()
				&& Subtypes.Count() == other.Subtypes.Count()
				&& Subtypes.Zip(other.Subtypes).All(i => i.First.ConstraintSatisfiedBy(i.Second));
		}

		public TypeSpecifier ApplyGeneric(TypeSpecifier other)
		{
			if (!ConstraintSatisfiedBy(other)) return this;

			var type = (TypeSpecifier)Clone();
			var typeMap = new Dictionary<string, TypeSpecifier>();

			type.ApplyGeneric(other, typeMap);

			return type;
		}

		protected virtual void ApplyGeneric(TypeSpecifier other, Dictionary<string, TypeSpecifier> typeMap)
		{
			foreach (var (first, second) in Subtypes.Zip(other.Subtypes))
			{
				first.ApplyGeneric(second, typeMap);
			}
		}

		public override int GetHashCode() => ToString().GetHashCode();

		public abstract override string ToString();
		protected abstract bool AreEqual(TypeSpecifier obj);
		public virtual TypeSpecifier GetEquatableType() => this;
		public abstract object Clone();

		public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
	}
}
