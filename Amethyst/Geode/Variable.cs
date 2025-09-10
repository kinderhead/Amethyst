using Amethyst.Geode.Values;

namespace Amethyst.Geode
{
    public class Variable(string name, string loc, TypeSpecifier type) : StorageValue(GeodeBuilder.RuntimeID, loc, type)
    {
        public readonly string Name = name;
        public override string ToString() => Name;
    }
}