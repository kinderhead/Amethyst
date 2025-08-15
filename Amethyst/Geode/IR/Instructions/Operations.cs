using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public abstract class OpInsn(ValueRef left, ValueRef right) : Simple2IntInsn<NBTInt>(left, right)
    {
        public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Int;
    }

    public class AddInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "add";
        public override NBTInt Compute(NBTInt left, NBTInt right) => left + right;
    }

    public class SubInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "sub";
        public override NBTInt Compute(NBTInt left, NBTInt right) => left - right;
    }

    public class MulInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "mul";
        public override NBTInt Compute(NBTInt left, NBTInt right) => left * right;
    }

    public class DivInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "div";
        public override NBTInt Compute(NBTInt left, NBTInt right) => left / right;
    }

    public class ModInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "mod";
        public override NBTInt Compute(NBTInt left, NBTInt right) => left % right;
    }
}
