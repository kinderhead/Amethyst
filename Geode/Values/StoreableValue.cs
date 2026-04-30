using System;

namespace Geode.Values
{
    public abstract class StoreableValue(TypeSpecifier type) : Value(type)
    {
        public abstract IValue AsStoreable();
    }
}
