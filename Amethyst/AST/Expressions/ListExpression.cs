using System;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
    public class ListExpression(LocationRange loc, List<Expression> exprs) : Expression(loc)
    {
        public readonly List<Expression> Expressions = exprs;

        protected override TypeSpecifier _ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(NBTType.List);

        protected override Value _Execute(FunctionContext ctx)
        {
            List<Value> values = [.. Expressions.Select(i => i.Execute(ctx))];

            if (values.FindIndex(i => i is not LiteralValue) == -1)
            {
                NBTList list = [.. values.Cast<LiteralValue>().Select(i => i.Value)];
                return new LiteralValue(list);
            }
            else
            {
                var tmp = ctx.AllocTemp(new PrimitiveTypeSpecifier(NBTType.List));
                Compute(ctx, tmp, values);
                return tmp;
            }
        }

        protected override void _Store(FunctionContext ctx, MutableValue dest)
        {
            List<Value> vals;
            if (dest.Type.PossibleInnerType is TypeSpecifier t) vals = [.. Expressions.Select(i => i.Cast(t).Execute(ctx))];
            else vals = [.. Expressions.Select(i => i.Execute(ctx))];
            Compute(ctx, dest, vals);
        }

        private void Compute(FunctionContext ctx, MutableValue dest, List<Value> values)
        {
            if (!dest.Type.IsList) throw new InvalidTypeError(Location, dest.Type.ToString());

            for (int i = 0; i < values.Count; i++)
            {
                values[i] = values[i].AsNotScore(ctx);
            }

            ctx.Add(new PopulateListInstruction(Location, (StorageValue)dest, values));
        }
    }
}
