using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Geode
{
	public abstract class TypeSpecifier : ICloneable
	{
		public abstract LiteralValue DefaultValue { get; }
		public abstract NamespacedID ID { get; }

		public virtual bool IsList => false;
		public virtual IEnumerable<TypeSpecifier> Subtypes => [];
		public virtual TypeSpecifier BaseClass => PrimitiveTypeSpecifier.Compound;

		// TODO: Make this not throw an error
		public virtual NBTType EffectiveType => throw new InvalidOperationException(ToString());
		public bool ShouldStoreInScore => EffectiveType is NBTType.Int or NBTType.Boolean;

		public NBTNumberType? EffectiveNumberType => Enum.IsDefined((NBTNumberType)EffectiveType) ? (NBTNumberType)EffectiveType : null;

		public virtual TypeSpecifier? DefaultPropertyType => null;
		public virtual Dictionary<string, TypeSpecifier> Properties => [];

		public TypeSpecifier? Property(string name)
		{
			if (Properties.TryGetValue(name, out var type))
			{
				return type;
			}
			else
			{
				return DefaultPropertyType;
			}
		}

		public virtual LiteralValue? DefaultPropertyValue(string name) => Property(name)?.DefaultValue;

		public override bool Equals(object? obj)
		{
			if (obj is not TypeSpecifier other)
			{
				return false;
			}
			else
			{
				var a = GetEquatableType();
				var b = other.GetEquatableType();
				if (a.GetType() != b.GetType())
				{
					return false; // Handle inheritance
				}
				else
				{
					return a.EqualsImpl(b);
				}
			}
		}

		public virtual bool Implements(TypeSpecifier other)
		{
			if (this == other)
			{
				return true;
			}
			else if (other.GetType() == GetType() && other.EffectiveType == EffectiveType && Subtypes.Count() == other.Subtypes.Count() && Subtypes.Zip(other.Subtypes).All(i => i.First.Implements(i.Second)))
			{
				return true;
			}
			else if (this == BaseClass)
			{
				return false;
			}
			else
			{
				return BaseClass.Implements(other);
			}
		}

		public virtual ValueRef ProcessArg(ValueRef src, FunctionContext ctx) => src;

		public virtual bool ConstraintSatisfiedBy(TypeSpecifier other) => other.GetType() == GetType()
				&& Subtypes.Count() == other.Subtypes.Count()
				&& Subtypes.Zip(other.Subtypes).All(i => i.First.ConstraintSatisfiedBy(i.Second));

		public TypeSpecifier ApplyGeneric(TypeSpecifier other)
		{
			if (!ConstraintSatisfiedBy(other))
			{
				return this;
			}

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

		public virtual TypeSpecifier AssignmentOverloadType => this;

		public virtual void AssignmentOverload(ValueRef dest, ValueRef val, FunctionContext ctx) => ctx.Add(new StoreInsn(dest, ctx.ImplicitCast(val, this)));

		public virtual ValueRef? ExplicitCastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx) => null;
		public virtual ValueRef? CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContext ctx) => null;
		public virtual ValueRef? CastToOverload(ValueRef val, FunctionContext ctx) => null;

		public override int GetHashCode() => ToString().GetHashCode();

		public abstract override string ToString();
		protected abstract bool EqualsImpl(TypeSpecifier obj);
		public virtual TypeSpecifier GetEquatableType() => this;
		public abstract object Clone();

		public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
		public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
	}
}
