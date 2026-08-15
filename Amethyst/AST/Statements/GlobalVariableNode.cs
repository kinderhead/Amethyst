using Datapack.Net.Utils;
using Geode;
using Geode.IR.Instructions;

namespace Amethyst.AST.Statements
{
    public class GlobalVariableNode(LocationRange loc, AbstractTypeSpecifier type, NamespacedID name, Expression? expr) : Node(loc), IRootChild
    {
        public readonly Expression? Expression = expr;
        public readonly NamespacedID Name = name;
        public readonly AbstractTypeSpecifier Type = type;

        public NamespacedID? Provides => null;

        public IEnumerable<NamespacedID> GetTypeDependencies(Compiler ctx) => Type.SoftResolve(ctx, Name.GetContainingFolder());

        public void Process(Compiler ctx, RootNode root)
        {
            var val = ctx.IR.AddGlobal(Name, Type.Resolve(ctx, Name.GetContainingFolder()), Location);
            if (Expression is not null) ctx.GlobalInitFunc.Add(new StoreInsn(val, Expression.Execute(ctx.GlobalInitFunc, val.Type)));
        }

        public void SecondPass(Compiler ctx, RootNode root)
        {
        }
    }
}