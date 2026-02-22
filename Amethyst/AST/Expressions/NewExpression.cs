using Geode;
using Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Expressions
{
	public class NewExpression(LocationRange loc, AbstractTypeSpecifier type, List<Expression> args) : Expression(loc)
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly List<Expression> Arguments = args;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var type = Type.Resolve(ctx);
			var ptr = ctx.Call("amethyst:gc/malloc").SetType(type);

			new CallExpression(Location, new VariableExpression(Location, type.ID.ToString()), [new ValueRefExpression(Location, ptr), .. Arguments]).Execute(ctx, null);

			return ptr;
		}
	}
}
