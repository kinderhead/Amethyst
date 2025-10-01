using Amethyst.Errors;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class PropertyInsn(ValueRef val, ValueRef prop, TypeSpecifier destType) : Instruction([val, prop])
    {
        public override string Name => "prop";
        public override NBTType?[] ArgTypes => [null, NBTType.String];
        public TypeSpecifier ActualReturnType => destType;
        public override TypeSpecifier ReturnType => new WeakReferenceTypeSpecifier(ActualReturnType);

        public override void Render(RenderContext ctx)
        {
            var val = Arg<ValueRef>(0).Expect();

            if (val.Type is not ReferenceTypeSpecifier && val is DataTargetValue nbt) val = WeakReferenceTypeSpecifier.From(nbt);

            ctx.Call("amethyst:core/ref/property", WeakReferenceTypeSpecifier.From(ReturnValue.Expect<DataTargetValue>()), val, Arg<ValueRef>(1));
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx)
        {
            var val = Arg<ValueRef>(0);
            var prop = Arg<ValueRef>(1);

            if (val.Type is not ReferenceTypeSpecifier && val.Value is DataTargetValue nbt && prop.Value is LiteralValue l)
            {
                if (l.Value is not NBTString name) throw new InvalidTypeError(prop.Type.ToString(), "string");
                Remove();
                return WeakReferenceTypeSpecifier.From(nbt.Property(name, ActualReturnType));
            }

            return null;
        }
    }
}
