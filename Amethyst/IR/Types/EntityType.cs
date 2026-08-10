using Amethyst.IR.Instructions;
using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Chains;
using Geode.IR;
using Geode.Types;
using Geode.Values;

namespace Amethyst.IR.Types
{
#pragma warning disable CS9107
    public class EntityType(
        NamespacedID id,
        TypeSpecifier? baseClass,
        Dictionary<string, TypeSpecifier> props,
        Dictionary<string, FunctionValue> methods) : StructType(id, baseClass, props, methods, false)
#pragma warning restore CS9107
    {
        public static readonly EntityType Dummy = new("amethyst:dummy", null, [], []);
        public override LiteralValue DefaultValue => new(0, this);
        public override NBTType EffectiveType => NBTType.Int;

#pragma warning disable IDE0028
        public override object Clone() => new EntityType(ID, BaseClass, new(props.Select(i => new KeyValuePair<string, TypeSpecifier>(i.Key, i.Value))), Methods);
#pragma warning restore IDE0028
        public override string ToString() => ID.ToString();
        protected override bool EqualsImpl(TypeSpecifier obj) => obj is EntityType other && other.ID == ID;

        public override void CastToOverload(ValueRef val, FunctionContextRecorder recorder)
        {
            switch (val.Type)
            {
                case TargetSelectorType:
                    recorder.Record(new EntityRefInsn(val));
                    break;
                case EntityType other when other.ID == "amethyst:dummy":
                    recorder.Record(val);
                    break;
                default:
                    base.CastToOverload(val, recorder);
                    break;
            }
        }

        public override void CastFromOverload(ValueRef val, TypeSpecifier to, FunctionContextRecorder recorder)
        {
            if (to is TargetSelectorType) recorder.Record(new EntityToTargetInsn(val));
            else base.CastFromOverload(val, to, recorder);
        }

        public override void ExecuteChainOverload(ValueRef val, ExecuteChain chain, FunctionContext ctx, bool invert = false) =>
            chain.Add(new IfEntityChain(ctx.ImplicitCast(val, new TargetSelectorType()), invert));
    }
}