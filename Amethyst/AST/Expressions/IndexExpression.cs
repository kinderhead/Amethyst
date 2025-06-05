using System;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;

namespace Amethyst.AST.Expressions
{
    public class IndexExpression(LocationRange loc) : Expression(loc)
    {
        protected override TypeSpecifier _ComputeType(FunctionContext ctx)
        {
            throw new NotImplementedException();
        }

        protected override Value _Execute(FunctionContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
