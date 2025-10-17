using System;

namespace Geode.Values
{
    public interface IAdvancedMacroValue
    {
        IConstantValue Macroize(Func<Value, IConstantValue> apply);
    }
}
