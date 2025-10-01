using Amethyst.Geode;
using Amethyst.Geode.IR;

namespace Amethyst.AST.Expressions
{
    public class CompoundExpression(LocationRange loc, IEnumerable<KeyValuePair<string, Expression>> values) : Expression(loc)
    {
        public readonly Dictionary<string, Expression> Values = new(values);

        // public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.Compound;

        protected override ValueRef _Execute(FunctionContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
