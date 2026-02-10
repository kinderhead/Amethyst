using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Values;
using System;

namespace Geode.Types
{
	public class SimpleMapType(TypeSpecifier inner) : TypeSpecifier
    {
        public readonly TypeSpecifier Inner = inner;
        public override NBTType EffectiveType => NBTType.Compound;
        public override IEnumerable<TypeSpecifier> Subtypes => [Inner];
        public override bool IsList => true;
        public override TypeSpecifier BaseClass => PrimitiveType.Compound;
        public override NamespacedID ID => "amethyst:map";
        public override LiteralValue DefaultValue => new(new NBTCompound(), this);

		public override TypeSpecifier? DefaultPropertyType => Inner;
		public override LiteralValue? DefaultPropertyValue(string name) => Inner.DefaultValue;
        
        protected override bool EqualsImpl(TypeSpecifier obj) => obj is SimpleMapType arr && arr.Inner == Inner;
        public override string ToString() => $"{Inner}{{}}";
        public override object Clone() => new SimpleMapType((TypeSpecifier)Inner.Clone());
    }
}
