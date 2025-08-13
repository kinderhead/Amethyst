using Amethyst.Geode;
using Amethyst.Geode.IR;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
    public class CompoundExpression(LocationRange loc, IEnumerable<KeyValuePair<string, Expression>> values) : Expression(loc)
    {
        public readonly Dictionary<string, Expression> Values = new(values);

        public override TypeSpecifier ComputeType(FunctionContext ctx) => PrimitiveTypeSpecifier.Compound;

        public override ValueRef Execute(FunctionContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
