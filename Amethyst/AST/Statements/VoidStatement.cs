using Geode;
using Geode.IR;

namespace Amethyst.AST.Statements
{
    public class VoidStatement() : Statement(LocationRange.None)
    {
        public override void Compile(FunctionContext ctx)
        {
        }
    }
}