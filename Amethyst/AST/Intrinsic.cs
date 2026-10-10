using Datapack.Net.Data;
using Datapack.Net.NumberProviders;
using Datapack.Net.Utils;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST
{
    public abstract class Intrinsic(NamespacedID id, FunctionType? type)
        : LiteralValue(new NBTString(id.ToString()), type ?? new(FunctionModifiers.None, new VoidType(), [])), IFunctionLike
    {
        public NamespacedID ID => id;

        public FunctionType FuncType => (FunctionType)Type;

        public abstract IFunctionLike CloneWithType(FunctionType type);

        public abstract ValueRef CallBehavior(FunctionContext ctx, params ValueRef[] args);
        public RawFunctionValue Get(TypeArray types) => throw new InvalidOperationException("Intrinsics do not have real functions");

        public override NumberProvider ToCompute(RenderContext ctx) => throw new ComputeError(this);

        public virtual Equation Compute(FunctionContext ctx, params Equation[] args) => throw new ComputeError(this);
    }
}