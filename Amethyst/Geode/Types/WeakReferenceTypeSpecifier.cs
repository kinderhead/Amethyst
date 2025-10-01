using Amethyst.Errors;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Values;

namespace Amethyst.Geode.Types
{
    public class WeakReferenceTypeSpecifier(TypeSpecifier inner) : ReferenceTypeSpecifier(inner)
    {
        public override string ToString() => $"{Inner}^";

        public override ValueRef? CastOverload(ValueRef val, FunctionContext ctx)
        {
            if (val.Type.Implements(Inner))
            {
                if (val.IsLiteral) throw new ReferenceError(val.Name);
                return ctx.Add(new WeakReferenceInsn(val));
            }
            if (val.Type is ReferenceTypeSpecifier r && r.Inner.Implements(Inner)) return val;
            else return base.CastOverload(val, ctx);
        }

        protected override bool EqualsImpl(TypeSpecifier obj) => obj is WeakReferenceTypeSpecifier p && p.Inner == Inner;
        public override object Clone() => new WeakReferenceTypeSpecifier((TypeSpecifier)Inner.Clone());

        public new static LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new WeakReferenceTypeSpecifier(val.Type));
    }
}
