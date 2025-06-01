using Amethyst.AST.Expressions;
using Amethyst.Codegen.IR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public abstract class Statement(LocationRange loc) : Node(loc)
	{
		public void Compile(FunctionContext ctx)
		{
			ctx.PushLocator(this);
			ctx.ClearTemps();
			try
			{
				_Compile(ctx);
			}
			catch (Exception) // Saw somewhere that it might be necessary to explicitly catch all errors so finally is run
			{
				throw;
			}
			finally
			{
				ctx.PopLocator();
			}
		}

		protected abstract void _Compile(FunctionContext ctx);
	}

	public class ExpressionStatement(LocationRange loc, Expression expr) : Statement(loc)
	{
		public readonly Expression Expression = expr;

		protected override void _Compile(FunctionContext ctx)
		{
			Expression.Execute(ctx);
		}
	}
}
