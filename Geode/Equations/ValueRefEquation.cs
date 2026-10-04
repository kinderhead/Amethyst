using Datapack.Net.Data;
using Datapack.Net.NumberProviders;
using Geode.Values;

namespace Geode.Equations
{
    public class ValueRefEquation(ValueRef val) : Equation([val], [])
    {
        public override NumberProvider Render(RenderContext ctx) => Values[0].Expect().ToCompute(ctx);
        public override NBTValue Execute(NBTValue[] args) => args[0];
        public override NBTValue? IsConstant() => (Values[0].Value as LiteralValue)?.Value;
        public override Equation Simplify() => this;
    }
}