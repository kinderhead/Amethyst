using System.Text;
using Amethyst.Errors;

namespace Amethyst.Geode.IR
{
    public class FunctionContext
    {
        public readonly Compiler Compiler;
        public readonly StaticFunctionValue Decl;

        public IReadOnlyList<Block> Blocks => blocks;

        private readonly List<Block> blocks = [];
        private readonly List<Scope> totalScopes = [];
        private readonly Stack<Scope> activeScopes = [];

        public IEnumerable<Variable> AllLocals => totalScopes.SelectMany(i => i.Locals.Values);

        public FunctionContext(Compiler compiler, StaticFunctionValue decl)
        {
            Compiler = compiler;
            Decl = decl;
            PushScope();
            blocks.Add(new("entry"));
        }

        public void PushScope()
        {
            var scope = new Scope();
            totalScopes.Add(scope);
            activeScopes.Push(scope);
        }

        public void PopScope()
        {
            activeScopes.Pop();
        }

        public Value GetLocal(string name)
        {
            foreach (var i in activeScopes.Reverse())
            {
                if (i.Locals.TryGetValue(name, out var variable)) return variable;
            }

            throw new UndefinedSymbolError(name);
        }

        public Variable RegisterLocal(string name, TypeSpecifier type)
        {
            var val = new Variable(name, type);
            activeScopes.Peek().Locals[name] = val;
            return val;
        }

        public void Add(Block block) => blocks.Add(block);
        public ValueRef Add(Instruction insn) => blocks.Last().Add(insn);

        public string Dump()
        {
            var builder = new StringBuilder();

            builder.AppendLine(((FunctionTypeSpecifier)Decl.Type).ToString(Decl.ID.ToString()) + " {");

            int valCounter = 0;
            Dictionary<ValueRef, int> valueMap = [];

            foreach (var i in blocks)
            {
                builder.Append(i.Dump(val =>
                {
                    if (val.Value is not null && val.Value.Name != "") return $"{(val.Value.IsLiteral ? "" : "%")}{val.Value.Name}";

                    if (!valueMap.TryGetValue(val, out var num))
                    {
                        num = valCounter++;
                        valueMap[val] = num;
                    }

                    return $"%{num}";
                }));
            }

            builder.Append("}\n");

            return builder.ToString();
        }

        private class Scope
        {
            public readonly Dictionary<string, Variable> Locals = [];
        }
    }
}
