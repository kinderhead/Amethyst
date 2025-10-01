using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
    public class ListExpression(LocationRange loc, List<Expression> exprs) : Expression(loc)
    {
        public readonly List<Expression> Expressions = exprs;

        // public override TypeSpecifier ComputeType(FunctionContext ctx)
        // {
        //     if (Expressions.Count == 0) return PrimitiveTypeSpecifier.List;

        //     var type = Expressions[0].ComputeType(ctx);

        //     foreach (var i in Expressions)
        //     {
        //         if (i.ComputeType(ctx) != type)
        //         {
        //             type = PrimitiveTypeSpecifier.List;
        //             break;
        //         }
        //     }

        //     return type;
        // }

        protected override ValueRef _Execute(FunctionContext ctx)
        {
            if (Expressions.Count == 0) return new LiteralValue(new NBTList());

            TypeSpecifier? type = null;
            List<ValueRef> vals = [];

            foreach (var i in Expressions)
            {
                var val = i.Execute(ctx);

                if (type is null) type = val.Type;
                else if (val.Type != type) type = PrimitiveTypeSpecifier.List;

                vals.Add(val);
            }

            throw new NotImplementedException();
        }
    }
}
