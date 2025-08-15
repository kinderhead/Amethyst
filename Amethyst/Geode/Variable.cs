namespace Amethyst.Geode
{
    public class Variable(string name, TypeSpecifier type) : LValue
    {
        public readonly string Name = name;
        public override TypeSpecifier Type => type;
        public override string ToString() => Name;
    }
}