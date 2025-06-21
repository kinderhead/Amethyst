using System;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
    public class CompoundExpression(LocationRange loc, IEnumerable<KeyValuePair<string, Expression>> values) : Expression(loc)
    {
        public readonly Dictionary<string, Expression> Values = new(values);

        protected override TypeSpecifier _ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(NBTType.Compound);

        protected override Value _Execute(FunctionContext ctx)
        {
            List<KeyValuePair<string, Value>> vals = [.. Values.Select(i => new KeyValuePair<string, Value>(i.Key, i.Value.Execute(ctx)))];
            // TODO: figure out a good way to differentiate constants from non constants before execution

            if (vals.FindIndex(i => i.Value.AsConstant() is null) == -1)
            {
                return new LiteralValue(new NBTCompound([.. vals.Select(i => new KeyValuePair<string, NBTValue>(i.Key, i.Value.AsConstant() ?? throw new InvalidOperationException()))]));
            }
            else
            {
                var tmp = ctx.AllocTemp(new PrimitiveTypeSpecifier(NBTType.Compound));
                ctx.Add(new PopulateInstruction(Location, tmp, new(vals)));
                return tmp;
            }
        }

        protected override void _Store(FunctionContext ctx, MutableValue dest)
        {
            List<KeyValuePair<string, Value>> vals = [.. Values.Select(i => new KeyValuePair<string, Value>(i.Key, i.Value.Execute(ctx)))];
            // TODO: figure out a good way to differentiate constants from non constants before execution

            if (vals.FindIndex(i => i.Value.AsConstant() is null) == -1)
            {
                dest.Store(ctx, new LiteralValue(new NBTCompound([.. vals.Select(i => new KeyValuePair<string, NBTValue>(i.Key, i.Value.AsConstant() ?? throw new InvalidOperationException()))])));
            }
            else
            {
                if (dest is not StorageValue s) throw new InvalidTypeError(Location, dest.Type.ToString());
                ctx.Add(new PopulateInstruction(Location, s, new(vals)));
            }
        }
    }
}
