using System.Collections.Immutable;
using Datapack.Net.Data;
using Datapack.Net.NumberProviders;
using Geode.Equations;
using Geode.IR;
using Geode.Values;

namespace Geode
{
    public abstract class Equation(IEnumerable<ValueRef> vals, IEnumerable<Equation> children) : IInstructionArg
    {
        public readonly Equation[] Children = [.. children];

        public readonly ValueRef[] Values = [.. vals];

        public abstract ProviderNumberType Type { get; }
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

        public virtual Equation Simplify()
        {
            for (var i = 0; i < Children.Length; i++)
            {
                Children[i] = Children[i].Simplify();
            }

            var args = new List<NBTValue>();
            foreach (var i in Children)
            {
                if (i.IsConstant() is { } val) args.Add(val);
                else return this;
            }

            return new ValueRefEquation(new LiteralValue(Execute([.. args])));
        }

        public abstract NumberProvider Render(RenderContext ctx);
        public abstract NBTValue Execute(NBTValue[] args);
        public abstract NBTValue? IsConstant();
    }
}