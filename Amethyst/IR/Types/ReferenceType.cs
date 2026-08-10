using Amethyst.Errors;
using Amethyst.IR.Chains;
using Amethyst.IR.Instructions;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Types
{
    public class ReferenceType(TypeSpecifier inner, bool mutable = true) : TypeSpecifier
    {
        public readonly TypeSpecifier Inner = inner;
        public readonly bool Mutable = mutable;

        public override IEnumerable<TypeSpecifier> Subtypes =>
            [Inner]; // Shouldn't need to unecessarily include the base subtypes here

        public override LiteralValue DefaultValue => new($"storage amethyst:runtime null.{Guid.NewGuid()}", this);
        public override NBTType EffectiveType => NBTType.String;
        public override TypeSpecifier BaseClass => this;
        public override NamespacedID ID => Mutable ? "amethyst:ref" : Inner.ID;
        public override bool WrapInQuotesForMacro => true;
        public override TypeSpecifier? DefaultPropertyType => Inner.DefaultPropertyType;
        public override IReadOnlyDictionary<string, TypeSpecifier> Properties => Inner.Properties;

        public virtual string Postfix => "&";

        public override TypeSpecifier AssignmentOverloadType => Inner;

        public override void AssignmentOverload(ValueRef dest, ValueRef val, FunctionContext ctx)
        {
            if (!Mutable || val.Value is NullValue)
                base.AssignmentOverload(dest, val, ctx);
            else if (val.Type is ReferenceType)
                ctx.Add(new StoreRefToRefInsn(dest, ctx.ImplicitCast(val, this)));
            else
                ctx.Add(new StoreRefInsn(dest, ctx.ImplicitCast(val, Inner)));
        }

        public override void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            if (val.Type.Implements(Inner))
            {
                if (val.IsLiteral) throw new ReferenceError(val.Name);
                if (val.Value is Variable v) recorder.Record(ctx => v.ToReference(ctx));
                else recorder.Record(new ReferenceInsn(val));
            }
            else if (val.Type is WeakReferenceType weak && weak.Inner.Implements(Inner)) recorder.Record(new ResolveWeakRefInsn(val));
        }

        public override void CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContextRecorder recorder)
        {
            if (Inner.Implements(to)) recorder.Record(ctx => Deref(val, ctx));
            else if (to is ReferenceType && Inner is VoidType) recorder.Record(val);
        }

        public override void ExplicitCastFromOverload(ValueRef val, TypeSpecifier to, FunctionContextRecorder recorder)
        {
            if (to.EffectiveType == NBTType.Int) recorder.Record(ctx => ctx.Add(new LoadInsn(Deref(val, ctx), to)));
            else if (to is ReferenceType r && r.Inner.Implements(Inner)) recorder.Record(val);
        }

        public override void ExecuteChainOverload(ValueRef val, ExecuteChain chain, FunctionContext ctx, bool invert = false) => chain.Add(new IfReferenceExists(val, invert));

        protected override bool EqualsImpl(TypeSpecifier obj) => obj is ReferenceType p && p.Inner == Inner;
        public override object Clone() => new ReferenceType((TypeSpecifier)Inner.Clone(), Mutable);

        public static ValueRef Deref(ValueRef src, FunctionContext ctx) => ctx.Add(new DereferenceInsn(src));

        public static LiteralValue From(DataTargetValue val) =>
            new(val.Target.GetTarget(), new ReferenceType(val.Type));

        public static ValueRef TryDeref(ValueRef src, FunctionContext ctx)
        {
            if (src.Type is ReferenceType r) return Deref(src, ctx);

            return src;
        }

        public override string ToString()
        {
            if (Inner is StructType { IsClass: true } s) return s.ToString();

            return $"{Inner}{Postfix}";
        }
    }
}