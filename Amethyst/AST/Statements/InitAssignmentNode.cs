using Amethyst.AST.Expressions;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class InitAssignmentNode(LocationRange loc, AbstractTypeSpecifier type, string name, Expression expr) : Statement(loc)
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly Expression Expression = expr;
		public override IEnumerable<Statement> Statements => [];

		protected override void _Compile(FunctionContext ctx)
		{
			if (ctx.Variables.TryGetValue(Name, out var sym)) throw new RedefinedSymbolError(Location, Name, sym.Location);

			var type = Type.Resolve(ctx, true);
			if (type is VoidTypeSpecifier) throw new InvalidTypeError(Location, "void");
			else if (type is VarTypeSpecifier) type = Expression.ComputeType(ctx);

			MutableValue val;
			if (type is PrimitiveTypeSpecifier p && p.Type != NBTType.Int || ctx.KeepLocalsOnStack) val = new StorageValue(new(Compiler.RuntimeID), $"stack[-1].{Name}", type);
			else val = ctx.AllocScore();
			ctx.Variables[Name] = new(Name, type, Location, val);
			Expression.Cast(type).Store(ctx, val);
		}
	}
}
