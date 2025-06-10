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
			var type = Type.Resolve(ctx, true);
			if (type is VoidTypeSpecifier) throw new InvalidTypeError(Location, "void");
			else if (type is VarTypeSpecifier) type = Expression.ComputeType(ctx);

			MutableValue val;
			if (!ctx.KeepLocalsOnStack && type is PrimitiveTypeSpecifier p && p.Type == NBTType.Int) val = ctx.AllocScore();
			else val = new StorageValue(new(Compiler.RuntimeID), ctx.GetStackVariablePath(Name), type);
			ctx.RegisterVariable(Name, val);
			Expression.Cast(type).Store(ctx, val);
		}
	}
}
