using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Geode
{
    public interface IMinimalFunction : IValue
    {
        ValueRef CallBehavior(FunctionContext ctx, params ValueRef[] args);
        RawFunctionValue Get(TypeArray types);
    }

    public interface IFunctionLike : IMinimalFunction
    {
        FunctionType FuncType { get; }

        IFunctionLike CloneWithType(FunctionType type);
    }
}