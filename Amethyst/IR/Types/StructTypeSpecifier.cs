using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Types
{
	public class StructTypeSpecifier(NamespacedID id, TypeSpecifier baseClass, Dictionary<string, TypeSpecifier> props, Dictionary<string, FunctionTypeSpecifier> methods) : TypeSpecifier
	{
		public override Dictionary<string, TypeSpecifier> Properties => new([.. props, .. BaseClass.Properties]);
		public readonly Dictionary<string, FunctionTypeSpecifier> Methods = methods;

		public override NBTType EffectiveType => BaseClass.EffectiveType;
		public override LiteralValue DefaultValue => new(new NBTCompound(Properties.Select(i => new KeyValuePair<string, NBTValue>(i.Key, DefaultPropertyValue(i.Key)?.Value ?? i.Value.DefaultValue.Value))), this);
		public override IEnumerable<TypeSpecifier> Subtypes => Properties.Select(i => i.Value);
		public override TypeSpecifier BaseClass => baseClass;

		public override NamespacedID ID => id;

		public FunctionTypeSpecifier? HierarchyMethod(string name)
		{
			if (BaseClass is StructTypeSpecifier s)
			{
				if (s.Methods.TryGetValue(name, out var type))
				{
					return type;
				}

				return s.HierarchyMethod(name);
			}

			return null;
		}

		public override LiteralValue? DefaultPropertyValue(string name)
		{
			if (Methods.TryGetValue(name, out var method))
			{
				return new(new NamespacedID($"{ID}/{name}").ToString(), method);
			}

			if (base.DefaultPropertyValue(name) is LiteralValue val)
			{
				return val;
			}

			return BaseClass.DefaultPropertyValue(name);
		}

		// TODO: Recursive generics

		public override bool ConstraintSatisfiedBy(TypeSpecifier other) => other == this;

		protected override void ApplyGeneric(TypeSpecifier other, Dictionary<string, TypeSpecifier> typeMap)
		{

		}

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is StructTypeSpecifier other && other.ID == ID;// && Properties.Count == other.Properties.Count && Properties.All(kv => other.Properties.TryGetValue(kv.Key, out var prop) && prop.Equals(kv.Value));
		public override string ToString() => ID.ToString();

		public override object Clone() => new StructTypeSpecifier(ID, BaseClass, new(props.Select(i => new KeyValuePair<string, TypeSpecifier>(i.Key, i.Value))), Methods);
	}
}
