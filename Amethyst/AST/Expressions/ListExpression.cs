using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;

namespace Amethyst.AST.Expressions
{
	public class ListExpression(LocationRange loc, List<Expression> exprs) : Expression(loc)
	{
		public readonly List<Expression> Expressions = exprs;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			if (Expressions.Count == 0)
			{
				return new LiteralValue(new NBTList());
			}

			TypeSpecifier? type = null;
			List<ValueRef> vals = [];

			foreach (var i in Expressions)
			{
				var val = i.Execute(ctx, null);

				if (type is null)
				{
					type = new ListTypeSpecifier(val.Type);
				}
				else if (new ListTypeSpecifier(val.Type) != type)
				{
					type = PrimitiveTypeSpecifier.List;
				}

				vals.Add(val);
			}

			return ctx.Add(new ListInsn(type!, vals));
		}
	}
}
