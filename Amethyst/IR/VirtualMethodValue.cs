using Amethyst.IR.Instructions;
using Amethyst.IR.Types;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR
{
    public class VirtualMethodValue(NamespacedID id, FunctionType type, LocationRange loc) : RawFunctionValue(id, type, loc)
    {
        public override IFunctionLike CloneWithType(FunctionType type) => new VirtualMethodValue(ID, type, Location);

        public override ValueRef CallBehavior(FunctionContext ctx, params ValueRef[] args)
        {
            var typeID = ReferenceType.TryDeref(ctx.Add(new PropertyInsn(args[0], new LiteralValue(StructType.TYPE_ID_PROPERTY), new UnsafeStringType())), ctx);
            var typeInfo = ctx.Add(new PropertyInsn(new(ctx.GetVariable("amethyst:type_info")), typeID, PrimitiveType.Compound, true));
            var func = ReferenceType.TryDeref(ctx.Add(new PropertyInsn(typeInfo, Raw($"methods.\"{ID.GetFile()}\""), FuncType)), ctx);

            ctx.Add(new PushFuncArgsInsn(FuncType, ctx.PrepArgs(FuncType, args)));
            return ctx.Add(new DynCallInsn(func));
        }
    }
}