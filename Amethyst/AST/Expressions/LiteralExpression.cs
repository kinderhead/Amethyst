using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
    public class LiteralExpression(LocationRange loc, NBTValue val) : Expression(loc)
    {
        public readonly NBTValue Value = val;

        protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
        {
            if (Value is NBTString str && !QStringType.InvalidQString().IsMatch(str.Value)) return new LiteralValue(Value, new QStringType());
            return new LiteralValue(Value);
        }
    }
}