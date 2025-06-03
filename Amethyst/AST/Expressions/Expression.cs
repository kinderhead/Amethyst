using Amethyst.AST.Statements;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Expressions
{
	public abstract class Expression(LocationRange loc) : Node(loc)
	{
		public TypeSpecifier ComputeType(FunctionContext ctx) => LocatorGuard(ctx, () => _ComputeType(ctx));
		public Value Execute(FunctionContext ctx) => LocatorGuard(ctx, () => _Execute(ctx));
		public void Store(FunctionContext ctx, MutableValue dest) => LocatorGuard(ctx, () => _Store(ctx, dest));

		public void Compare(FunctionContext ctx, ExecuteWrapper truthy) => LocatorGuard(ctx, () => _Compare(ctx, truthy));

		public void LocatorGuard(FunctionContext ctx, Action cb) => LocatorGuard(ctx, () =>
		{
			cb();
			return 0;
		});

		public T LocatorGuard<T>(FunctionContext ctx, Func<T> cb)
		{
			ctx.PushLocator(this);
			try
			{
				return cb();
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

		public virtual Expression Cast(TypeSpecifier type) => new CastExpression(Location, type, this);

		protected abstract TypeSpecifier _ComputeType(FunctionContext ctx);
		protected abstract Value _Execute(FunctionContext ctx);

		protected virtual void _Store(FunctionContext ctx, MutableValue dest)
		{
			dest.Store(ctx, Execute(ctx));
		}

		protected virtual void _Compare(FunctionContext ctx, ExecuteWrapper truthy)
		{
			var val = Execute(ctx);

			if (val is LiteralValue l)
			{
				if (l.ToScoreInt() == 0) truthy.NeverExecute = true;
				//else falsy.NeverExecute = true;
				return;
			}

			var score = val.AsScore(ctx);

			//falsy.Cmd.If.Score(score.Target, score.Score, 0);
			truthy.Cmd.Unless.Score(score.Target, score.Score, 0);
		}
	}

	public class LiteralExpression(LocationRange loc, NBTValue val) : Expression(loc)
	{
		public readonly NBTValue Value = val;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => new PrimitiveTypeSpecifier(Value.Type);
		protected override Value _Execute(FunctionContext ctx) => new LiteralValue(Value);
	}

	public class VariableExpression(LocationRange loc, string name) : Expression(loc)
	{
		public readonly string Name = name;

		protected override TypeSpecifier _ComputeType(FunctionContext ctx) => ctx.GetVariable(Name).Type;
		protected override Value _Execute(FunctionContext ctx) => ctx.GetVariable(Name);
	}

	public class ExecuteWrapper(Execute? cmd = null)
	{
		public readonly Execute Cmd = cmd ?? new();

		private bool neverExecute = false;
		public bool NeverExecute { get => neverExecute; set => neverExecute = neverExecute || value; }
	}
}
