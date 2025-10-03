using Amethyst.Errors;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
    public class CompoundExpression(LocationRange loc, IEnumerable<KeyValuePair<string, Expression>> values) : Expression(loc)
    {
        public readonly Dictionary<string, Expression> Values = new(values);

        protected override ValueRef _Execute(FunctionContext ctx, TypeSpecifier? expected)
		{
            var type = expected ?? PrimitiveTypeSpecifier.Compound;

            SortedDictionary<string, ValueRef> vals = [];

			foreach (var (k, v) in Values)
			{
                if (type.Property(k) is not TypeSpecifier t) throw new PropertyError(type.ToString(), k);
                vals[k] = v.Execute(ctx, t);
            }

			foreach (var (k, _) in type.Properties)
			{
                if (!vals.ContainsKey(k) && type.DefaultPropertyValue(k) is Value v) vals[k] = v;
			}

			return ctx.Add(new CompoundInsn(vals, type));
        }
    }
}
