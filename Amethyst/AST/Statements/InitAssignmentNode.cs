using Amethyst.AST.Expressions;
using Amethyst.Errors;
using Amethyst.IR.Types;
using Geode;
using Geode.IR;
using Geode.IR.Instructions;
using Geode.Types;
using Geode.Values;

namespace Amethyst.AST.Statements
{
	[Flags]
	public enum StorageModifiers
	{
		None = 0,
		Const = 1,
		Static = 2,
	}

	public class InitAssignmentNode(LocationRange loc, StorageModifiers mod, AbstractTypeSpecifier type, string name, Expression? expr) : Statement(loc)
	{
		public readonly StorageModifiers Modifiers = mod;
		public readonly AbstractTypeSpecifier Type = type;
		public readonly string Name = name;
		public readonly Expression? Expression = expr;

		public override void Compile(FunctionContext ctx)
		{
			var type = Type.Resolve(ctx, Expression is not null);
			var val = Expression is null ? type.DefaultValue : Expression.Execute(Modifiers.HasFlag(StorageModifiers.Static) ? ((Compiler)ctx.Compiler).GlobalInitFunc : ctx, type is VarType ? null : type);

			if (type is VarType && Expression is not null)
			{
				type = val.Type;
			}
			else if (Expression is null)
			{
				if (ctx.GetConstructorOrNull(type) is not null)
				{
					throw new MissingConstructorError(type.ToString());
				}
			}

			if (Modifiers.HasFlag(StorageModifiers.Const))
			{
				if (Expression is null || val.Value is not IConstantValue c)
				{
					throw new ConstantValueError();
				}

				ctx.RegisterLocal(Name, c, Location);
			}
			else if (Modifiers.HasFlag(StorageModifiers.Static))
			{
				var dest = ctx.Compiler.IR.AddGlobal($"{ctx.Decl.ID}/\"@{Name}\"", type, Location, "static");
				ctx.RegisterLocal(Name, dest, Location);

				if (Expression is not null)
				{
					((Compiler)ctx.Compiler).GlobalInitFunc.Add(new StoreInsn(dest, val));
				}
			}
			else
			{
				var dest = ctx.RegisterLocal(Name, type, Location);
				ctx.Add(new StoreInsn(dest, val));
			}
		}
	}
}
