using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Expressions
{
	public class ReferenceExpression(LocationRange loc, Expression inner) : Expression(loc)
	{
		public readonly Expression Inner = inner;

		public override TypeSpecifier ComputeType(FunctionContext ctx) => new PointerTypeSpecifier(Inner.ComputeType(ctx));
        protected override ValueRef _Execute(FunctionContext ctx) => ctx.Add(new ReferenceInsn(Inner.Execute(ctx)));
    }
}
