using System.Text;
using Amethyst.Errors;
using Amethyst.Geode.IR.Instructions;

namespace Amethyst.Geode.IR
{
    public class FunctionContext
    {
        public readonly Compiler Compiler;
        public readonly StaticFunctionValue Decl;

        public Block FirstBlock => blocks.First();
        public readonly Block ExitBlock = new("exit");
        public Block CurrentBlock { get; private set; }

        public bool IsFinished => CurrentBlock == ExitBlock;

        public IReadOnlyCollection<Block> Blocks => blocks;
        private readonly List<Block> blocks = [];
        private readonly List<Scope> totalScopes = [];
        private readonly Stack<Scope> activeScopes = [];
        private readonly Dictionary<string, int> labelCounters = [];

        public IEnumerable<Variable> AllLocals => totalScopes.SelectMany(i => i.Locals.Values);

        public FunctionContext(Compiler compiler, StaticFunctionValue decl)
        {
            Compiler = compiler;
            Decl = decl;

            PushScope();

            blocks.Add(new("entry"));
            CurrentBlock = blocks.Last();
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

        public ValueRef Add(Instruction insn, string? customName = null)
        {
            if (IsFinished) throw new InvalidOperationException("Function is finished");
            var val = CurrentBlock.Add(insn);
            val.SetCustomName(customName);
            return val;
        }

        public void Finish()
        {
            CurrentBlock.Link(ExitBlock);
            CurrentBlock = ExitBlock;
        }

        public string GetNewLabelName(string label)
        {
            if (!labelCounters.TryGetValue(label, out var count))
            {
                labelCounters[label] = 1;
                return label;
            }

            labelCounters[label] = count + 1;
            return $"{label}{count}";
        }

        /// <summary>
        /// Branch current block without an else statement
        /// </summary>
        /// <param name="cond">Condition</param>
        /// <param name="label">Label name</param>
        /// <param name="ifTrue">If true action</param>
        /// <returns>True block</returns>
        public Block Branch(ValueRef cond, string label, Action ifTrue)
        {
            label = GetNewLabelName(label);

            var startingBlock = CurrentBlock;
            var trueBlock = new Block($"{label}.true");
            var endBlock = new Block($"{label}.end");

            blocks.Add(trueBlock);
            blocks.Add(endBlock);

            startingBlock.Link(trueBlock);
            startingBlock.Link(endBlock);
            trueBlock.Link(endBlock);

            Add(new BranchInsn(cond, trueBlock, endBlock));

            CurrentBlock = trueBlock;
            ifTrue();
            Add(new JumpInsn(endBlock));
            CurrentBlock = endBlock;

            return trueBlock;
        }

        public (Block trueBlock, Block falseBlock) Branch(ValueRef cond, string label, Action ifTrue, Action ifFalse)
        {
            label = GetNewLabelName(label);

            var startingBlock = CurrentBlock;
            var trueBlock = new Block($"{label}.true");
            var falseBlock = new Block($"{label}.false");
            var endBlock = new Block($"{label}.end");

            blocks.Add(trueBlock);
            blocks.Add(falseBlock);
            blocks.Add(endBlock);

            startingBlock.Link(trueBlock);
            startingBlock.Link(falseBlock);
            trueBlock.Link(endBlock);
            falseBlock.Link(endBlock);

            Add(new BranchInsn(cond, trueBlock, falseBlock));

            CurrentBlock = trueBlock;
            ifTrue();
            Add(new JumpInsn(endBlock));

            CurrentBlock = falseBlock;
            ifFalse();
            Add(new JumpInsn(endBlock));

            CurrentBlock = endBlock;

            return (trueBlock, falseBlock);
        }

        public string Dump()
        {
            var builder = new StringBuilder();

            builder.AppendLine(((FunctionTypeSpecifier)Decl.Type).ToString(Decl.ID.ToString()) + " {");

            int valCounter = 0;
            Dictionary<IInstructionArg, int> valueMap = [];

            foreach (var i in blocks)
            {
                if (i.Instructions.Count == 0) continue;

                builder.AppendLine(i.Dump(val =>
                {
                    if (val.Name != "") return val.Name;

                    if (!valueMap.TryGetValue(val, out var num))
                    {
                        num = valCounter++;
                        valueMap[val] = num;
                    }

                    return $"%{num}";
                }));
            }

            builder.Length--;
            builder.Append("}\n");

            return builder.ToString();
        }

        private class Scope
        {
            public readonly Dictionary<string, Variable> Locals = [];
        }
    }
}
