using Amethyst.Errors;
using Datapack.Net.Function;
using Geode;
using Geode.Errors;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Util;

namespace Amethyst.AST.Expressions
{
	public class TargetSelectorExpression(LocationRange loc, TargetType type, MultiDictionary<string, Expression> args) : Expression(loc)
	{
		public readonly TargetType Type = type;
		public readonly MultiDictionary<string, Expression> Arguments = args;

		protected override ValueRef ExecuteImpl(FunctionContext ctx, TypeSpecifier? expected)
		{
			var args = new MultiDictionary<string, ValueRef>();

			foreach (var (k, v) in Arguments)
			{
				var effectiveName = k;
				var effectiveValue = v;

				if (v is NotExpression n)
                {
                    if (k is "tag" or "team" or "gamemode" or "name" or "type" or "family" or "nbt" or "predicate")
                    {
                        effectiveName = '!' + k;
						effectiveValue = n.Value;
                    }
					else
                    {
                        throw new CannotNegateArgumentError(k);
                    }
                }

				// Maybe change this to switch on the desired type
				var val = k switch
				{
					"x" or "y" or "z" or "dx" or "dy" or "dz" => effectiveValue.Execute(ctx, PrimitiveType.Double),
					"limit" => effectiveValue.Execute(ctx, PrimitiveType.Int),
					"name" or "type" or "predicate" => effectiveValue.Execute(ctx, PrimitiveType.String),
					"tag" or "team" or "sort" or "gamemode" => effectiveValue.Execute(ctx, new UnsafeStringType()),
					"distance" or "x_rotation" or "y_rotation" => effectiveValue.Execute(ctx, new FloatRangeType()),
					"level" => effectiveValue.Execute(ctx, new IntRangeType()),
					"nbt" => effectiveValue.Execute(ctx, PrimitiveType.Compound),
					_ => throw new TargetSelectorArgumentError(k),
				};

				args.Add(new(effectiveName, val));
			}

			return ctx.Add(new TargetSelectorInsn(Type, args));
		}
	}
}
