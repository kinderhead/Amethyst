using Datapack.Net.Function;
using Geode;
using Geode.IR;

namespace Amethyst.AST.Expressions
{
    public class TargetSelectorExpression(LocationRange loc, TargetType type, Dictionary<string, Expression> args) : Expression(loc)
    {
        public readonly TargetType Type = type;
        public readonly Dictionary<string, Expression> Arguments = args;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            throw new NotImplementedException();
        }
    }
}
