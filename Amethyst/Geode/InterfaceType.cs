using Datapack.Net.Data;

namespace Amethyst.Geode
{
    public class InterfaceType(Dictionary<string, ObjectProperty> props) : TypeSpecifier
    {
        public readonly Dictionary<string, ObjectProperty> Properties = props;

        public override bool Operable => false;
        public override LiteralValue DefaultValue => new(new NBTCompound(), this);
        public override IEnumerable<TypeSpecifier> Subtypes => Properties.Select(i => i.Value.Type);

        protected override bool AreEqual(TypeSpecifier obj) => obj is InterfaceType other && Properties.Count == other.Properties.Count && Properties.All(kv => other.Properties.TryGetValue(kv.Key, out var prop) && prop.Equals(kv.Value));
        public override string ToString() => $"interface {{\n{string.Join('\n', Properties.Select(i => $"    {i.Value.Type} {i.Key};"))}\n}}";
        //public override bool IsAssignableTo(TypeSpecifier other) => (other is InterfaceType i && i.Properties.All(kv => Properties.TryGetValue(kv.Key, out var prop) && prop.Equals(kv.Value))) || base.IsAssignableTo(other);
        public override TypeSpecifier? Property(string name) => Properties.TryGetValue(name, out var prop) ? prop.Type : base.Property(name);

        public override object Clone() => new InterfaceType(new(Properties.Select(i => new KeyValuePair<string, ObjectProperty>(i.Key, new((TypeSpecifier)i.Value.Type.Clone(), i.Value.Name)))));
    }

    public readonly record struct ObjectProperty(TypeSpecifier Type, string Name);
}
