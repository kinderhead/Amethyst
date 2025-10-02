using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.AST.Expressions
{
    public class CompoundExpression(LocationRange loc, IEnumerable<KeyValuePair<string, Expression>> values) : Expression(loc)
    {
        public readonly Dictionary<string, Expression> Values = new(values);

        protected override ValueRef _Execute(FunctionContext ctx)
        {
            return ctx.Add(new CompoundInsn(new(new Dictionary<string, ValueRef>(Values.Select(i => new KeyValuePair<string, ValueRef>(i.Key, i.Value.Execute(ctx)))))));
        }
    }
}
