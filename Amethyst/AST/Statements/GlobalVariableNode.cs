using Amethyst.AST.Expressions;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.Values;
using Datapack.Net.Utils;

namespace Amethyst.AST.Statements
{
	public class GlobalVariableNode(LocationRange loc, AbstractTypeSpecifier type, NamespacedID name, Expression? expr) : Node(loc), IRootChild
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly NamespacedID Name = name;
		public readonly Expression? Expression = expr;

		public void Process(Compiler ctx, RootNode root)
		{
			var val = new StorageValue(new NamespacedID(Name.Namespace, "globals"), Name.Path, Type.Resolve(ctx, Name.GetContainingFolder()));
			ctx.IR.AddSymbol(new(Name, Location, val));

			if (Expression is not null)
			{
				ctx.GlobalInitFunc.Add(new StoreInsn(val, Expression.Execute(ctx.GlobalInitFunc, val.Type)));
			}
		}
	}
}
