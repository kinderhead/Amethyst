using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Chains;
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
        public abstract NBTType EffectiveType { get; }

        public virtual bool IsList => false;
        public virtual IEnumerable<TypeSpecifier> Subtypes => [];
        public virtual TypeSpecifier BaseClass => PrimitiveType.Compound;
        public virtual bool WrapInQuotesForMacro => false;

        public bool ShouldStoreInScore => EffectiveType is NBTType.Int or NBTType.Boolean;

        public NBTNumberType? EffectiveNumberType => Enum.IsDefined((NBTNumberType)EffectiveType) ? (NBTNumberType)EffectiveType : null;

        public virtual TypeSpecifier? DefaultPropertyType => null;
        public virtual IReadOnlyDictionary<string, TypeSpecifier> Properties => new Dictionary<string, TypeSpecifier>();

        public virtual TypeSpecifier AssignmentOverloadType => this;
        public abstract object Clone();

        public TypeSpecifier? HasProperty(string name, bool allowDefault = false)
        {
            if (Properties.TryGetValue(name, out var type)) return type;

            return allowDefault ? DefaultPropertyType : null;
        }

        public virtual LiteralValue? DefaultPropertyValue(string name) => HasProperty(name)?.DefaultValue;

        public override bool Equals(object? obj)
        {
            if (obj is not TypeSpecifier other) return false;

            var a = GetEquatableType();
            var b = other.GetEquatableType();
            if (a.GetType() != b.GetType()) return false; // Handle inheritance

            return a.EqualsImpl(b);
        }

        public virtual bool Implements(TypeSpecifier other)
        {
            if (this == other) return true;

            if (other.GetType() == GetType() && other.EffectiveType == EffectiveType && Subtypes.Count() == other.Subtypes.Count() &&
                Subtypes.Zip(other.Subtypes).All(i => i.First.Implements(i.Second)))
                return true;

            if (this == BaseClass) return false;

            return BaseClass.Implements(other);
        }

        public virtual bool ConstraintSatisfiedBy(TypeSpecifier other) => other.GetType() == GetType() && Subtypes.Count() == other.Subtypes.Count() &&
                                                                          Subtypes.Zip(other.Subtypes).All(i => i.First.ConstraintSatisfiedBy(i.Second));

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

        public virtual void AssignmentOverload(ValueRef dest, ValueRef val, FunctionContext ctx) => ctx.Add(new StoreInsn(dest, ctx.ImplicitCast(val, this)));

        public virtual void ExplicitCastFromOverload(ValueRef val, TypeSpecifier to, FunctionContextRecorder recorder)
        {
        }

        public virtual void ExplicitCastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
        }

        public virtual void CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContextRecorder recorder)
        {
        }

        public virtual void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
        }

        public virtual void ExecuteChainOverload(ValueRef val, ExecuteChain chain, FunctionContext ctx, bool invert = false) => chain.Add(IfValueChain.With(val, ctx, invert));

        public override int GetHashCode() => ToString().GetHashCode();

        public abstract override string ToString();
        protected abstract bool EqualsImpl(TypeSpecifier obj);
        public virtual TypeSpecifier GetEquatableType() => this;

        public static bool operator ==(TypeSpecifier a, TypeSpecifier b) => a.Equals(b);
        public static bool operator !=(TypeSpecifier a, TypeSpecifier b) => !a.Equals(b);
    }

    public partial class TypeArray(IEnumerable<TypeSpecifier> types)
    {
        public readonly ImmutableArray<TypeSpecifier> Types = [.. types];
        public int Length => Types.Length;
        public TypeSpecifier this[int i] => Types[i];

        public NamespacedID Mangle(NamespacedID id) => id.WithFile($"_{id.GetFile()}-{ResourceLocationRegex().Replace(ToString(), "_")}").ToLower();

        public override bool Equals(object? obj) => obj is TypeArray other && Types.SequenceEqual(other.Types);

        public override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (var i in Types)
            {
                hash.Add(i);
            }

            return hash.ToHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var i in Types)
            {
                sb.Append($"{i}, ");
            }

            if (Length != 0) sb.Length -= 2;

            return sb.ToString();
        }


        public static bool operator ==(TypeArray a, TypeArray b) => a.Equals(b);
        public static bool operator !=(TypeArray a, TypeArray b) => !a.Equals(b);

        public static TypeArray From(IEnumerable<IValueLike> args) => new(args.Select(i => i.Type));

        [GeneratedRegex(@"[^a-zA-Z0-9\-_]")]
        private static partial Regex ResourceLocationRegex();
    }
}