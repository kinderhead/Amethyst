using Geode;
using Geode.IR;
using System;

namespace Amethyst.AST
{
    public interface IExecuteChainExpression
    {
        public void ExecuteChain(ExecuteChain chain, FunctionContext ctx);
    }
}
