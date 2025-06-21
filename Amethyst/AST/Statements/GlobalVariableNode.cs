using System;
using Amethyst.AST.Expressions;
using Amethyst.Codegen;
using Amethyst.Errors;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

namespace Amethyst.AST.Statements
{
    public class GlobalVariableNode(LocationRange loc, AbstractTypeSpecifier type, NamespacedID name, Expression? expr) : Node(loc)
    {
        public readonly AbstractTypeSpecifier Type = type;
        public readonly NamespacedID Name = name;
        public readonly Expression? Expression = expr;

        public void Process(Compiler ctx)
        {
            LiteralValue? val = null;
            if (Expression is not null)
            {
                if (Expression.Execute(ctx.FauxCtx(Expression.Location)) is not LiteralValue l) throw new MutableValueError(Location);
                val = l;
            }

            var type = Type.Resolve(ctx, true);
            if (type is VoidTypeSpecifier) throw new InvalidTypeError(Location, "void");
            else if (type is VarTypeSpecifier)
            {
                if (val is null) throw new InvalidTypeError(Location, "var");
                type = val.Type;
            }

            var sym = new StorageValue(new(Name.Namespace + ":globals"), Name.Path, type);
            ctx.Register(sym.Storage, sym.Path);

            if (ctx.Symbols.TryGetValue(Name, out var original)) throw new RedefinedSymbolError(Location, Name.ToString(), original.Location);
            ctx.Symbols[Name] = new(Name, Location, sym, this);

            if (val is not null) ctx.UserInitCommands.Add(new Execute().Unless.Data(sym.Storage, sym.Path).Run(val.ReadTo(sym)));
        }
    }
}
