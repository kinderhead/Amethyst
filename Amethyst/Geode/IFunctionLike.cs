using Amethyst.Geode.IR;
using Amethyst.Geode.Types;

namespace Amethyst.Geode
{
    public interface IFunctionLike
    {
        public FunctionTypeSpecifier FuncType { get; }

        public IFunctionLike CloneWithType(FunctionTypeSpecifier type);
        public ValueRef AsMethod(ValueRef self, FunctionContext ctx);
    }
}
