﻿using Amethyst.AST.Expressions;
using Amethyst.Errors;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Statements
{
	public class InitAssignmentNode(LocationRange loc, AbstractTypeSpecifier type, string name, Expression? expr, bool careAboutConstructors = true) : Statement(loc)
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly Expression? Expression = expr;
		public readonly bool CareAboutConstructors = careAboutConstructors;

		public override void Compile(FunctionContext ctx)
		{
			var type = Type.Resolve(ctx, Expression is not null);
			var val = Expression is null ? type.DefaultValue : Expression.Execute(ctx, type);

			if (type is VarTypeSpecifier && Expression is not null)
			{
				type = val.Type;
			}
			else if (Expression is null)
			{
				if (CareAboutConstructors && ctx.GetConstructorOrNull(type) is not null)
				{
					throw new MissingConstructorError(type.ToString());
				}
			}

			var dest = ctx.RegisterLocal(Name, type);

			ctx.Add(new StoreInsn(dest, val));
		}
	}
}
