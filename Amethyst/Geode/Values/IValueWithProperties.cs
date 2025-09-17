namespace Amethyst.Geode.Values
{
    public interface IValueWithProperties<T> where T : Value
    {
        public T Property(string prop, TypeSpecifier type);
    }
}
