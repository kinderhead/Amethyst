using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Types
{
	public class StructType(NamespacedID id, TypeSpecifier? baseClass, Dictionary<string, TypeSpecifier> props, Dictionary<string, FunctionValue> methods, bool isClass) : TypeSpecifier
	{
#pragma warning disable IDE0028 // Simplify collection initialization
		public override Dictionary<string, TypeSpecifier> Properties => new([.. props, .. BaseClass == this ? [] : BaseClass.Properties]);
#pragma warning restore IDE0028 // Simplify collection initialization

		public readonly Dictionary<string, FunctionValue> Methods = methods;
		public readonly bool IsClass = isClass;

		public override NBTType EffectiveType => BaseClass.EffectiveType;
		public override LiteralValue DefaultValue => new(new NBTCompound(Properties.Select(i => new KeyValuePair<string, NBTValue>(i.Key, DefaultPropertyValue(i.Key)?.Value ?? i.Value.DefaultValue.Value))), this);
		public override IEnumerable<TypeSpecifier> Subtypes => Properties.Select(i => i.Value);
		public override TypeSpecifier BaseClass => baseClass ?? this;

		public override NamespacedID ID => id;

		public (StructType Source, FunctionValue Function)? HierarchyMethod(string name)
		{
			if (BaseClass is StructType s && BaseClass != this)
			{
				if (s.Methods.TryGetValue(name, out var type))
				{
					return (this, type);
				}

				return s.HierarchyMethod(name);
			}

			return null;
		}

		public override LiteralValue? DefaultPropertyValue(string name)
		{
			if (BaseClass is StructType && BaseClass.DefaultPropertyValue(name) is LiteralValue b)
			{
				return b;
			}
			else if (base.DefaultPropertyValue(name) is LiteralValue val)
			{
				return val;
			}

			return null;
		}
#pragma warning disable IDE0028 // Simplify collection initialization

		public NBTCompound GetTypeInfo() =>
			new([
				new("methods", new NBTCompound([.. Methods.Select(i => new KeyValuePair<string, NBTValue>(i.Key, i.Value.ID.ToString()))])),
				new("properties", new NBTCompound([.. Properties.Select(i => new KeyValuePair<string, NBTValue>(i.Key, i.Value.ToString()))])),
				new("base", BaseClass.ID.ToString()),
			]);

		public LiteralValue DefaultValueWithMetadata => new(new NBTCompound([.. Properties.Select(i => new KeyValuePair<string, NBTValue>(i.Key, DefaultPropertyValue(i.Key)?.Value ?? i.Value.DefaultValue.Value)), new(TypeIDProperty, ID.ToString())]), this);

		// TODO: Recursive generics
		public override bool ConstraintSatisfiedBy(TypeSpecifier other) => other == this;

		// TODO: This
		protected override void ApplyGeneric(TypeSpecifier other, Dictionary<string, TypeSpecifier> typeMap) { }

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is StructType other && other.ID == ID;// && Properties.Count == other.Properties.Count && Properties.All(kv => other.Properties.TryGetValue(kv.Key, out var prop) && prop.Equals(kv.Value));
		public override string ToString() => ID.ToString();

		public override object Clone() => new StructType(ID, BaseClass, new(props.Select(i => new KeyValuePair<string, TypeSpecifier>(i.Key, i.Value))), Methods, IsClass);

#pragma warning restore IDE0028 // Simplify collection initialization

		public static readonly string TypeIDProperty = "@type";
	}
}
