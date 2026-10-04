using System.Collections.Immutable;
using Datapack.Net.NumberProviders;
using Geode.IR;

namespace Geode
{
    public abstract class Equation(IEnumerable<ValueRef> vals, IEnumerable<Equation> children) : IInstructionArg
    {
        public readonly Equation[] Children = [.. children];

        public readonly ValueRef[] Values = [.. vals];
        public string Name => "equation";
        public IReadOnlySet<ValueRef> Dependencies => ImmutableHashSet.Create([.. Values, .. Children.SelectMany(i => i.Dependencies)]);

        public void ReplaceValue(ValueRef value, ValueRef with)
        {
            for (var i = 0; i < Values.Length; i++)
            {
                if (Values[i] == value) Values[i] = with;
            }

            foreach (var i in Children)
            {
                i.ReplaceValue(value, with);
            }
        }

        public abstract NumberProvider Render(RenderContext ctx);
    }
}