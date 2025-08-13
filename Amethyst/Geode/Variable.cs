namespace Amethyst.Geode
{
    public class Variable(string name, TypeSpecifier type) : Value
    {
        public override string Name => name;
        public override TypeSpecifier Type => type;
    }
}