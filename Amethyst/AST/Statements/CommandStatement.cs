using Amethyst.AST.Expressions;
using Amethyst.Geode;
using Amethyst.Geode.IR;
using Amethyst.Geode.IR.Instructions;
using System.Text;

namespace Amethyst.AST.Statements
{
	public class CommandStatement(LocationRange loc, IEnumerable<CommandFragment> cmd) : Statement(loc)
	{
		public readonly CommandFragment[] Fragments = [..cmd];
		public override IEnumerable<Statement> SubStatements => [];

		public override void Compile(FunctionContext ctx)
		{
			var cmd = new StringBuilder();
			int arg = 0;

			foreach (var i in Fragments)
			{
				i.Resolve(ctx);
				cmd.Append(i.Render(ref arg));
            }

			if (arg == 0) ctx.Add(new CommandInsn(cmd.ToString()));
			else
			{
				var (cmdCtx, func) = ctx.Compiler.AnonymousFunction(new(FunctionModifiers.None, new VoidTypeSpecifier(), Fragments.Where(i => i is CommandExprFragment).Cast<CommandExprFragment>().Select(i => new Parameter(ParameterModifiers.Macro, i.Value.Type, $"arg{i.ArgIndex}"))));
				
				cmdCtx.Add(new CommandInsn(cmd.ToString()));
				cmdCtx.Add(new ReturnInsn());
				cmdCtx.Finish();

                ctx.Add(new CallInsn(func, Fragments.Where(i => i is CommandExprFragment).Cast<CommandExprFragment>().Select(i => i.Value)));
            }
        }
	}

	public abstract class CommandFragment
	{
		public abstract void Resolve(FunctionContext ctx);
		public abstract string Render(ref int arg);
	}

	public class CommandTextFragment(string text) : CommandFragment
	{
		public readonly string Text = text;

		public override string Render(ref int arg) => Text;
		public override void Resolve(FunctionContext ctx) { }
	}

	public class CommandExprFragment(Expression expr) : CommandFragment
	{
		public readonly Expression Expression = expr;
		public ValueRef Value { get => field ?? throw new InvalidOperationException(); private set; }
		public int ArgIndex { get; private set; } = -1;

        public override string Render(ref int arg) => Value.IsLiteral && Value.Value is Value v ? v.ToString() : $"$(arg{ArgIndex = arg++})";

		public override void Resolve(FunctionContext ctx)
		{
			Value = Expression.Execute(ctx);
        }
	}
}
