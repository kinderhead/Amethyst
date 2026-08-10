using Amethyst.Errors;
using Amethyst.IR.Instructions;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.Values;

namespace Amethyst.IR.Types
{
    public class WeakReferenceType(TypeSpecifier inner) : ReferenceType(inner)
    {
        public override string Postfix => "^";
        public override NamespacedID ID => "amethyst:weak_ref";

        public override void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (val.Type.Implements(Inner))
            {
                if (val.IsLiteral) throw new ReferenceError(val.Name);
                if (val.Value is Variable v) v.HasReference = true;

                recorder.Record(new WeakReferenceInsn(val));
            }
            else if (val.Type is ReferenceType r && r.Inner.Implements(Inner)) recorder.Record(val);
            else base.CastToOverload(val, recorder);
        }

        protected override bool EqualsImpl(TypeSpecifier obj) => obj is WeakReferenceType p && p.Inner == Inner;
        public override object Clone() => new WeakReferenceType((TypeSpecifier)Inner.Clone());

        public new static LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new WeakReferenceType(val.Type));
    }
}