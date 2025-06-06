using System;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
    public class IndexExpression(LocationRange loc, Expression list, Expression index) : Expression(loc)
    {
        public readonly Expression List = list;
        public readonly Expression Index = index;

        protected override TypeSpecifier _ComputeType(FunctionContext ctx)
        {
            var type = List.ComputeType(ctx);
            if (type is not ListTypeSpecifier lType) throw new InvalidTypeError(Location, type.ToString());
            return lType.Inner;
        }

        protected override Value _Execute(FunctionContext ctx)
        {
            return SubExecute(ctx, null);
        }

        protected override void _Store(FunctionContext ctx, MutableValue dest)
        {
            if (dest is StorageValue s) SubExecute(ctx, s);
            else dest.Store(ctx, SubExecute(ctx, null));
        }

        private StorageValue SubExecute(FunctionContext ctx, StorageValue? dest)
        {
            var arr = List.Execute(ctx);
            var ind = Index.Cast(new PrimitiveTypeSpecifier(NBTType.Int)).Execute(ctx);

            if (arr is not StorageValue arrStorage || arr.Type is not ListTypeSpecifier lType) throw new InvalidTypeError(Location, arr.Type.ToString());

            if (ind is LiteralValue literalInd)
            {
                var val = new StorageValue(arrStorage.Storage, arrStorage.Path + $"[{literalInd.Value}]", lType.Inner);
                dest?.Store(ctx, val);
                return val;
            }
            else
            {
                arrStorage = arr.AsStorage(ctx);
                dest ??= ctx.AllocTemp(lType.Inner);
                var args = new Dictionary<string, Value>
                {
                    ["src_storage"] = new LiteralValue(arrStorage.Storage.ToString()),
                    ["src_path"] = new LiteralValue(arrStorage.Path),
                    ["dest_storage"] = new LiteralValue(dest.Storage.ToString()),
                    ["dest_path"] = new LiteralValue(dest.Path),
                    ["index"] = ind
                };
                ctx.Add(new CallInstruction(Location, "amethyst:core/index-list", [], args));
                return dest;
            }
        }
    }
}
