namespace Geode.Values
{
    public abstract class StorableValue(TypeSpecifier type) : Value(type)
    {
        public abstract IValue AsStoreable();
    }
}