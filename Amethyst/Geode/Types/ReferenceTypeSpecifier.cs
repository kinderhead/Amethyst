using Amethyst.Errors;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.Geode.Types
{
    public class ReferenceTypeSpecifier(TypeSpecifier inner) : TypeSpecifier
    {
        public readonly TypeSpecifier Inner = inner;
        public override IEnumerable<TypeSpecifier> Subtypes => [Inner]; // Shouldn't need to unecessarily include the base subtypes here
        public override bool Operable => false;
        public override LiteralValue DefaultValue => new("");
        public override string ToString() => $"{Inner}&";
        public override NBTType EffectiveType => NBTType.String;
        public override string BasePath => "amethyst";
		public override TypeSpecifier BaseClass => this;

        // public override ValueRef ProcessArg(ValueRef src, FunctionContext ctx) => From(src);

        public override void AssignmentOverload(ValueRef dest, ValueRef val, FunctionContext ctx)
        {
            ctx.Call("amethyst:core/ref/set", dest, val);
            // ctx.Add(new StoreInsn(dest, ctx.ImplicitCast(val, Inner)));
        }

		public override ValueRef? CastOverload(ValueRef val, FunctionContext ctx)
		{
			if (val.Type.Implements(Inner))
			{
                if (val.IsLiteral) throw new ReferenceError(val.Name);
                return ctx.Add(new ReferenceInsn(val));
            }

            return null;
        }

        public override TypeSpecifier? Property(string name) => Inner.Property(name);

        protected override bool AreEqual(TypeSpecifier obj) => obj is ReferenceTypeSpecifier p && p.Inner == Inner;
        public override object Clone() => new ReferenceTypeSpecifier((TypeSpecifier)Inner.Clone());

        public static LiteralValue From(DataTargetValue val) => new(val.Target.GetTarget(), new ReferenceTypeSpecifier(val.Type));
        public static ValueRef From(ValueRef src)
        {
            var ptr = new ValueRef(new ReferenceValue(src));
            ptr.Dependencies.Add(src);
            return ptr;
        }

    }
}
