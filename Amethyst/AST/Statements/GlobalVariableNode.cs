using Datapack.Net.Utils;
using Geode;
using Geode.IR.Instructions;
using Geode.Values;

namespace Amethyst.AST.Statements
{
	public class GlobalVariableNode(LocationRange loc, AbstractTypeSpecifier type, NamespacedID name, Expression? expr) : Node(loc), IRootChild
	{
		public readonly AbstractTypeSpecifier Type = type;
		public readonly NamespacedID Name = name;
		public readonly Expression? Expression = expr;

		public void Process(Compiler ctx, RootNode root)
		{
			var val = ctx.IR.AddGlobal(Name, Type.Resolve(ctx, Name.GetContainingFolder()), Location);

			if (Expression is not null)
			{
				ctx.GlobalInitFunc.Add(new StoreInsn(val, Expression.Execute(ctx.GlobalInitFunc, val.Type)));
			}
		}
	}
}
