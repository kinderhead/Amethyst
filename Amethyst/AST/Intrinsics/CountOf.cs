using Amethyst.AST.Expressions;
using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions.Utils;
using Datapack.Net.Data;

namespace Amethyst.AST.Intrinsics
{
    public class CountOf() : Intrinsic("countOf")
    {
        public override TypeSpecifier Type => new FunctionTypeSpecifier(FunctionModifiers.None, PrimitiveTypeSpecifier.Int, [new(ParameterModifiers.None, PrimitiveTypeSpecifier.String, "id")]);

        public override ValueRef Execute(FunctionContext ctx, params IEnumerable<Expression> args)
        {
            if (args.Count() != 1) throw new MismatchedArgumentCountError(1, args.Count());
            var arg = args.First().Execute(ctx);
            if (arg.Expect<LiteralValue>().Value is not NBTString id) throw new InvalidTypeError(arg.Type.ToString(), "constant string");
            return ctx.Add(new CountOfInsn(id.Value));
        }
    }
}
