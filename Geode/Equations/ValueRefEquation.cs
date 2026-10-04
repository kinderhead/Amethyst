using Datapack.Net.NumberProviders;

namespace Geode.Equations
{
    public class ValueRefEquation(ValueRef val) : Equation([val], [])
    {
        public override NumberProvider Render(RenderContext ctx) => Values[0].Expect().ToCompute(ctx);
    }
}