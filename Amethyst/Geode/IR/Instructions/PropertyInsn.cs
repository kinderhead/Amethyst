using Amethyst.Errors;
using Datapack.Net.Data;

namespace Amethyst.Geode.IR.Instructions
{
    public class PropertyInsn(ValueRef val, ValueRef prop, TypeSpecifier destType) : Instruction([val, prop])
    {
        public override string Name => "prop";
        public override NBTType?[] ArgTypes => [null, NBTType.String];
        public override TypeSpecifier ReturnType => destType;

        public override void Render(RenderContext ctx)
        {
            throw new NotImplementedException();
        }

        protected override Value? ComputeReturnValue(FunctionContext ctx)
        {
            var val = Arg<ValueRef>(0);
            var prop = Arg<ValueRef>(1);

            if (val.Value is StorageValue sv && prop.Value is LiteralValue l)
            {
                if (l.Value is not NBTString name) throw new InvalidTypeError(prop.Type.ToString(), "string");
                Remove();
                return sv.Property(name, ReturnType);
            }

            return null;
        }
    }
}
