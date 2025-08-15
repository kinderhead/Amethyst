using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public abstract class OpInsn(ValueRef left, ValueRef right) : Simple2IntInsn<NBTInt>(left, right)
    {
        public override TypeSpecifier ReturnType => PrimitiveTypeSpecifier.Int;
        public abstract bool IsCommunitive { get; }

        public override void CheckPotentialScoreReuse(Func<ValueRef, ValueRef, bool> tryLink)
        {
            if (Arguments[0] is ValueRef v0 && tryLink(ReturnValue, v0)) return;
            else if (IsCommunitive && Arguments[1] is ValueRef v1)
            {
                tryLink(ReturnValue, v1);
                Arguments[1] = Arguments[0];
                Arguments[0] = v1;
            }
        }
    }

    public class AddInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "add";
        public override bool IsCommunitive => true;
        public override NBTInt Compute(NBTInt left, NBTInt right) => left + right;
    }

    public class SubInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "sub";
        public override bool IsCommunitive => false;
        public override NBTInt Compute(NBTInt left, NBTInt right) => left - right;
    }

    public class MulInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "mul";
        public override bool IsCommunitive => true;
        public override NBTInt Compute(NBTInt left, NBTInt right) => left * right;
    }

    public class DivInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "div";
        public override bool IsCommunitive => false;
        public override NBTInt Compute(NBTInt left, NBTInt right) => left / right;
    }

    public class ModInsn(ValueRef left, ValueRef right) : OpInsn(left, right)
    {
        public override string Name => "mod";
        public override bool IsCommunitive => false;
        public override NBTInt Compute(NBTInt left, NBTInt right) => left % right;
    }
}
