using Datapack.Net.Data;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Expressions
{
	public class ListExpression(LocationRange loc, List<Expression> exprs) : Expression(loc)
	{
		public readonly List<Expression> Expressions = exprs;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			if (Expressions.Count == 0)
			{
				return new LiteralValue(new NBTList(), expected);
			}

			TypeSpecifier? type = null;
			List<ValueRef> vals = [];

			foreach (var i in Expressions)
			{
				var val = i.Execute(ctx, null);

				if (type is null)
				{
					type = new ListType(val.Type);
				}
				else if (new ListType(val.Type) != type)
				{
					type = PrimitiveType.List;
				}

				vals.Add(val);
			}

			return ctx.Add(new ListInsn(type!, vals));
		}
	}
}
