using Amethyst.AST.Expressions;
using Amethyst.Geode.IR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.AST.Statements
{
	public class ForStatement(LocationRange loc, Statement? initializer, Expression condition, Expression iterator, Statement body) : Statement(loc)
	{
		public readonly Statement? Initializer = initializer;
		public readonly Expression Condition = condition;
		public readonly Expression Iterator = iterator;
		public readonly Statement Body = body;

        public override void Compile(FunctionContext ctx)
		{
			Initializer?.Compile(ctx);
			ctx.Loop(() => Condition.Execute(ctx), "for", () => {
				Body.Compile(ctx);
				Iterator.Execute(ctx);
            });
        }
	}
}
