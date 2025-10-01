using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;

namespace Amethyst.AST.Expressions
{
    public class IndexExpression(LocationRange loc, Expression list, Expression index) : Expression(loc)
    {
        public readonly Expression List = list;
        public readonly Expression Index = index;

        // public override TypeSpecifier ComputeType(FunctionContext ctx)
        // {
        //     var type = List.ComputeType(ctx);
        //     if (!type.IsList || !type.Subtypes.Any()) throw new InvalidTypeError(type.ToString());
        //     return type.Subtypes.First();
        // }

        protected override ValueRef _Execute(FunctionContext ctx)
        {
            return ctx.Add(new IndexInsn(List.Execute(ctx), ctx.ImplicitCast(Index.Execute(ctx), PrimitiveTypeSpecifier.Int)));
        }
    }
}
