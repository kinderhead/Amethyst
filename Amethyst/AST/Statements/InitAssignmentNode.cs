using Amethyst.AST.Expressions;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class InitAssignmentNode(LocationRange loc, AbstractTypeSpecifier type, string name, Expression expr) : AbstractStatement(loc)
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly Expression Expression = expr;

		protected override void _Compile(FunctionContext ctx)
		{
			if (ctx.Variables.TryGetValue(Name, out var sym)) throw new RedefinedSymbolError(Location, Name, sym.Location);

			var type = Type.Resolve(ctx);
			if (type is VoidTypeSpecifier) throw new InvalidTypeError(Location, "void");

			ctx.Variables[Name] = new(Name, type, Location, new(new(Compiler.RuntimeID), $"stack[-1].{Name}", type));
			ctx.Variables[Name].Value.Store(ctx, Expression.Cast(ctx, type));
		}
	}
}
