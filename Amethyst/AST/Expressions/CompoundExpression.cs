using Amethyst.Errors;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
	public class CompoundExpression(LocationRange loc, IEnumerable<KeyValuePair<string, Expression>> values) : Expression(loc)
	{
		public readonly Dictionary<string, Expression> Values = new(values);

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var type = expected ?? PrimitiveType.Compound;

			if (ctx.GetConstructorOrNull(type) is not null)
			{
				throw new MissingConstructorError(type.ToString());
			}

			SortedDictionary<string, ValueRef> vals = [];

			foreach (var (k, v) in Values)
			{
				if (type.HasProperty(k) is not TypeSpecifier t)
				{
					throw new PropertyError(type.ToString(), k);
				}

				vals[k] = v.Execute(ctx, t);
			}

			foreach (var (k, _) in type.Properties)
			{
				if (!vals.ContainsKey(k) && type.DefaultPropertyValue(k) is LiteralValue v)
				{
					vals[k] = v;
				}
			}

			return ctx.Add(new CompoundInsn(vals, type));
		}
	}
}
