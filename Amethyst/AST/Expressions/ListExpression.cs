using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
    public class ListExpression(LocationRange loc, List<Expression> exprs) : Expression(loc)
    {
        public readonly List<Expression> Expressions = exprs;

        public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.List;

        public override ValueRef Execute(FunctionContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
