namespace Amethyst.Geode
{
    public class Variable(string name, TypeSpecifier type) : StorageValue(GeodeBuilder.RuntimeID, $"stack[-1].{name}", type)
    {
        public readonly string Name = name;
        public override string ToString() => Name;
    }
}