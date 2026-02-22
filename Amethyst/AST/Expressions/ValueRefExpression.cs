using Geode;
using Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Expressions
{
	public class ValueRefExpression(LocationRange loc, ValueRef val) : Expression(loc)
	{
		public readonly ValueRef Value = val;
		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected) => Value;
	}
}
