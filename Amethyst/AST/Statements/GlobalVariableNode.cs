using Amethyst.AST.Expressions;
using Datapack.Net.Utils;

namespace Amethyst.AST.Statements
{
    public class GlobalVariableNode(LocationRange loc, AbstractTypeSpecifier type, NamespacedID name, Expression? expr) : Node(loc), IRootChild
    {
        public readonly AbstractTypeSpecifier Type = type;
        public readonly NamespacedID Name = name;
        public readonly Expression? Expression = expr;

        public void Process(Compiler ctx)
        {
            throw new NotImplementedException();
        }
    }
}
