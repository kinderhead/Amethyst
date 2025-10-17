using Datapack.Net.Function;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;

namespace Amethyst.AST.Expressions
{
	public class TargetSelectorExpression(LocationRange loc, TargetType type, Dictionary<string, Expression> args) : Expression(loc)
	{
		public readonly TargetType Type = type;
		public readonly Dictionary<string, Expression> Arguments = args;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var args = new SortedDictionary<string, ValueRef>();

			foreach (var (k, v) in Arguments)
			{
				args[k] = k switch
				{
					"x" or "y" or "z" or "dx" or "dy" or "dz" => v.Execute(ctx, PrimitiveType.Double),
					"limit" => v.Execute(ctx, PrimitiveType.Int),
					"name" or "type" => v.Execute(ctx, PrimitiveType.String),
					_ => throw new TargetSelectorArgumentError(k),
				};
			}

			return ctx.Add(new TargetSelectorInsn(Type, args));
		}
	}
}
