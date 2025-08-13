using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
    public class IndexExpression(LocationRange loc, Expression list, Expression index) : Expression(loc)
    {
        public readonly Expression List = list;
        public readonly Expression Index = index;

        public override TypeSpecifier ComputeType(FunctionContext ctx)
        {
            var type = List.ComputeType(ctx);
            if (type is not ListTypeSpecifier lType) throw new InvalidTypeError(type.ToString());
            return lType.Inner;
        }

        public override ValueRef Execute(FunctionContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
