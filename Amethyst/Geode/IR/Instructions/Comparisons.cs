using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public abstract class ComparisonInsn(ValueRef left, ValueRef right) : Simple2IntInsn<NBTBool>(left, right)
    {
        public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Bool;

        public override void Render(RenderContext ctx)
        {
            throw new NotImplementedException();
        }
    }

    public class EqInsn(ValueRef left, ValueRef right) : ComparisonInsn(left, right)
    {
        public override string Name => "eq";
        public override NBTBool Compute(NBTInt left, NBTInt right) => left == right;
    }
}
