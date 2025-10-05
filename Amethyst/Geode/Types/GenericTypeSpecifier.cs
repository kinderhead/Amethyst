using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Amethyst.Geode.Types
{
	public class GenericTypeSpecifier(string name, TypeSpecifier? constraint = null, bool resolved = false) : TypeSpecifier
	{
		public readonly string Name = name;
		public TypeSpecifier Constraint { get; private set; } = constraint ?? PrimitiveTypeSpecifier.Compound;
		public bool Resolved { get; private set; } = resolved;

		public override LiteralValue DefaultValue => Constraint.DefaultValue;
		public override bool IsList => Constraint.IsList;
		public override NamespacedID ID => Constraint.ID;
		public override TypeSpecifier BaseClass => Constraint;
		public override NBTType EffectiveType => Constraint.EffectiveType;
		public override IEnumerable<TypeSpecifier> Subtypes => Constraint.Subtypes;

		// Also kinda does the GetHashCode as well because I did a silly for that
		public override string ToString() => Resolved ? Constraint.ToString() : Name;

		public override bool ConstraintSatisfiedBy(TypeSpecifier other) => other.Implements(Constraint);
		public override bool Implements(TypeSpecifier other) => Constraint.Implements(other);

		public void Set(TypeSpecifier type)
		{
			Constraint = type;
			Resolved = true;
		}

		protected override void ApplyGeneric(TypeSpecifier other, Dictionary<string, TypeSpecifier> typeMap)
		{
			if (!Resolved)
			{
				if (typeMap.TryGetValue(Name, out var type))
				{
					other = type;
				}

				Set(other);
				typeMap[Name] = other;
			}
			else if (other != typeMap[Name])
			{
				throw new NotImplementedException(); // Maybe remove this
			}

			base.ApplyGeneric(other, typeMap);
		}

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is GenericTypeSpecifier other && other.Name == Name && other.Constraint == Constraint;
		public override TypeSpecifier GetEquatableType() => Resolved ? Constraint.GetEquatableType() : this;
		public override int GetHashCode() => HashCode.Combine(Name, Constraint);
		public override object Clone() => new GenericTypeSpecifier(Name, Constraint, Resolved);
	}
}
