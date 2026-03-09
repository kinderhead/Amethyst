using System;

namespace Geode.Values
{
    public abstract class StoreableValue : Value
    {
        public abstract IValue AsStoreable();
    }
}
