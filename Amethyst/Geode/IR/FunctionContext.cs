using System.Text;
using Amethyst.Errors;
using Amethyst.Geode.IR.Instructions;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

namespace Amethyst.Geode.IR
{
    public class FunctionContext
    {
        public readonly Compiler Compiler;
        public readonly StaticFunctionValue Decl;
        public readonly IEnumerable<NamespacedID> Tags;

        public Block FirstBlock => blocks.First();
        public readonly Block ExitBlock;
        public Block CurrentBlock { get; private set; }

        public bool IsFinished => CurrentBlock == ExitBlock;

        public IReadOnlyCollection<Block> Blocks => blocks;
        private readonly List<Block> blocks = [];
        private readonly List<Scope> totalScopes = [];
        private readonly Stack<Scope> activeScopes = [];
        private readonly Dictionary<string, int> labelCounters = [];

        public IEnumerable<Variable> AllLocals => totalScopes.SelectMany(i => i.Locals.Values);

        public FunctionContext(Compiler compiler, StaticFunctionValue decl, IEnumerable<NamespacedID> tags)
        {
            Compiler = compiler;
            Decl = decl;
            Tags = tags;

            PushScope();

            blocks.Add(new("entry", Decl.ID));
            CurrentBlock = blocks.Last();

            ExitBlock = new("exit", GetNewInternalID());
        }

        public FunctionContext(Compiler compiler, StaticFunctionValue decl) : this(compiler, decl, []) { }

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
            var val = new Variable(name, $"frame{activeScopes.Count - 1}.{name}", type);
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

        public ValueRef ImplicitCast(ValueRef val, TypeSpecifier type)
        {
            if (val.Type != type) throw new InvalidTypeError(val.Type.ToString(), type.ToString());
            return val;
        }

        public void Finish()
        {
            //FirstBlock.InsertAtBeginning(AllLocals.Select(i => new DeclareInsn(i)));
            if (CurrentBlock.Instructions.Count == 0 || !CurrentBlock.Instructions.Last().IsReturn) throw new InvalidOperationException("Last block in function must have a return instruction");
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
            var trueBlock = new Block($"{label}.true", GetNewInternalID());
            var endBlock = new Block($"{label}.end", GetNewInternalID());

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
            var trueBlock = new Block($"{label}.true", GetNewInternalID());
            var falseBlock = new Block($"{label}.false", GetNewInternalID());
            var endBlock = new Block($"{label}.end", GetNewInternalID());

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

        public void Render(GeodeBuilder builder)
        {
            if (AllLocals.Any()) FirstBlock.Function.Add(new DataCommand.Modify(GeodeBuilder.RuntimeID, "stack").Append().Value("{}"));

            foreach (var i in blocks)
            {
                i.Render(builder, this);
            }

            builder.Unregister(ExitBlock.Function);
			//if (AllLocals.Any()) FirstBlock.Function.Add(new DataCommand.Remove(GeodeBuilder.RuntimeID, "stack[-1]"));

			foreach (var i in Tags)
			{
                builder.Datapack.Tags.GetTag(i, "function").Values.Add(Decl.ID);
			}
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

        public NamespacedID GetNewInternalID() => new(Decl.ID.Namespace, $"zz_internal/{GeodeBuilder.RandomString}");

        // This is separate from the global NBT return value so that it can handle branch conclusions.
        // If a better way is found, then it won't have to move NBT at return.
        public StorageValue GetFunctionReturnValue() => new(GeodeBuilder.RuntimeID, "stack[-1].$ret", Decl.FuncType.ReturnType);

        private class Scope
        {
            public readonly Dictionary<string, Variable> Locals = [];
        }
    }
}
