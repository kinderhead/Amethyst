using System.Text;
using Amethyst.AST;
using Amethyst.Errors;
using Amethyst.Geode.IR.Instructions;
using Amethyst.Geode.IR.Passes;
using Amethyst.Geode.Types;
using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;

namespace Amethyst.Geode.IR
{
    public class FunctionContext
    {
        public readonly Compiler Compiler;
        public readonly FunctionValue Decl;
        public readonly IEnumerable<NamespacedID> Tags;

        public readonly Stack<LocationRange> LocationStack = [];

        public Block FirstBlock => blocks.First();
        public readonly Block ExitBlock;
        public Block CurrentBlock { get; private set; }

        public bool IsFinished => CurrentBlock == ExitBlock;
        public bool UsesStack => AllLocals.Any(i => i is StorageValue) || registersInUse.Count != 0 || tmpStackVars != 0;
        public readonly bool IsMacroFunction;
        public readonly bool HasTagPriority;

        public IReadOnlyCollection<Block> Blocks => blocks;
        private readonly List<Block> blocks = [];
        private readonly List<Scope> totalScopes = [];
        private readonly Stack<Scope> activeScopes = [];
        private readonly Dictionary<string, int> labelCounters = [];

        // Could just be the max register used, but who knows if one goes poof somewhere
        private readonly HashSet<int> registersInUse = [];
        private int tmpStackVars = 0;

        public IEnumerable<Value> AllLocals => totalScopes.SelectMany(i => i.Locals.Values);

        public FunctionContext(Compiler compiler, FunctionValue decl, IEnumerable<NamespacedID> tags, bool hasTagPriority = false)
        {
            Compiler = compiler;
            Decl = decl;
            Tags = tags;
            IsMacroFunction = Decl.FuncType.Parameters.Any(i => i.Modifiers.HasFlag(ParameterModifiers.Macro));
            HasTagPriority = hasTagPriority;

            PushScope();

            blocks.Add(new("entry", Decl.ID, this));
            CurrentBlock = blocks.Last();

            ExitBlock = new("exit", GetNewInternalID(), this);

            foreach (var i in Decl.FuncType.Parameters)
            {
                if (i.Modifiers.HasFlag(ParameterModifiers.Macro))
                {
                    RegisterLocal(i.Name, new MacroValue(i.Name, i.Type));
                }
                else
                {
                    // Maybe make it so that if the stack isn't used by the function, then use -1 and don't push new frame
                    RegisterLocal(i.Name, new StackValue(-2, $"args.{i.Name}", i.Type));
                }
            }
        }

        public FunctionContext(Compiler compiler, FunctionValue decl) : this(compiler, decl, []) { }

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

        public Value GetVariable(string name) => GetVariableOrNull(name) ?? throw new UndefinedSymbolError(name);

        public Value? GetVariableOrNull(string name)
        {
            foreach (var i in activeScopes.Reverse())
            {
                if (i.Locals.TryGetValue(name, out var variable)) return variable;
            }

            if (GetGlobal(new(Decl.ID.ContainingFolder(), name)) is Value v) return v;
            if (GetGlobal(new("builtin", name)) is Value v2) return v2;

            return null;
        }

        public Value? GetGlobal(NamespacedID id)
        {
            if (Compiler.Symbols.TryGetValue(id, out var sym)) return sym.Value;
            return null;
        }

        public ValueRef GetProperty(ValueRef val, string name)
        {
            if (GetGlobal(new(val.Type.BasePath, name)) is FunctionValue func && func.FuncType.Parameters.Length >= 1)
            {
                func = func.CloneWithType(func.FuncType.ApplyGenericWithParams([new ReferenceTypeSpecifier(val.Type)]));
                var firstArgType = func.FuncType.Parameters[0].Type;
                if (firstArgType is ReferenceTypeSpecifier r && val.Type.Implements(r.Inner)) return new MethodValue(func, ImplicitCast(val, firstArgType));
            }
            else if (val.Type.Property(name) is TypeSpecifier t) return Add(new PropertyInsn(val, new LiteralValue(name), t));

            throw new PropertyError(val.Type.ToString(), name);
        }

        public Variable RegisterLocal(string name, TypeSpecifier type) => RegisterLocal(name, $"frame{activeScopes.Count - 1}.{name}", type);

        public Variable RegisterLocal(string name, string loc, TypeSpecifier type)
        {
            var val = new Variable(name, loc, type);
            RegisterLocal(name, val);
            return val;
        }

        public void RegisterLocal(string name, Value val) => activeScopes.Peek().Locals[name] = val;

        public StackValue Temp(TypeSpecifier type) => new(-1, $"tmp{tmpStackVars++}", type);

        public ValueRef Add(Instruction insn, string? customName = null)
        {
            if (IsFinished) throw new InvalidOperationException("Function is finished");

            if (LocationStack.Count != 0) insn.Location = LocationStack.Peek();
            var val = CurrentBlock.Add(insn);
            val.SetCustomName(customName);
            return val;
        }

        public ValueRef ImplicitCast(ValueRef val, TypeSpecifier type)
        {
            if (ImplicitCastOrNull(val, type) is ValueRef ret) return ret;
            throw new InvalidTypeError(val.Type.ToString(), type.ToString());
        }

        public ValueRef? ImplicitCastOrNull(ValueRef val, TypeSpecifier type)
        {
            if (val.Type == type) return val;
            else if (type is VarTypeSpecifier) return val;
            else if (type is AnyTypeSpecifier) return val.SetType(type);
            else if (val.Type is AnyTypeSpecifier) return val.SetType(type); // This hopefully shouldn't cause problems with reusing values
            else if (val.Type.Implements(type)) return val.SetType(type);
            else if (val.Type is ReferenceTypeSpecifier r2 && r2.Inner.Implements(type)) return ReferenceTypeSpecifier.From(val, this).SetType(type);
            else if (val.Value is LiteralValue literal && type is PrimitiveTypeSpecifier)
            {
                if (literal.Value.NumberType is NBTNumberType && type.EffectiveNumberType is NBTNumberType destType)
                {
                    return new LiteralValue(literal.Value.Cast(destType));
                }
            }
            else if (type.CastOverload(val, this) is ValueRef cast) return cast.SetType(type);

            return null;
        }

        public ValueRef ExplicitCast(ValueRef val, TypeSpecifier type)
        {
            if (ImplicitCastOrNull(val, type) is ValueRef ret) return ret;
            else if (type.EffectiveType == NBTType.Int) return Add(new LoadInsn(val, type));

            throw new InvalidTypeError(val.Type.ToString(), type.ToString());
        }

        public ValueRef Call(NamespacedID id, params ValueRef[] args) => Call(GetGlobal(id) as FunctionValue ?? throw new UndefinedSymbolError(id.ToString()), args);

        public ValueRef Call(FunctionValue f, params ValueRef[] args)
        {
            if (f.FuncType.Parameters.Length != args.Length) throw new MismatchedArgumentCountError(f.FuncType.Parameters.Length, args.Length);
            return Add(new CallInsn(f, args.Zip(f.FuncType.Parameters).Select(i => ImplicitCast(i.First, i.Second.Type))));
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
            var trueBlock = new Block($"{label}.true", GetNewInternalID(), this);
            var endBlock = new Block($"{label}.end", GetNewInternalID(), this);

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
            var trueBlock = new Block($"{label}.true", GetNewInternalID(), this);
            var falseBlock = new Block($"{label}.false", GetNewInternalID(), this);
            var endBlock = new Block($"{label}.end", GetNewInternalID(), this);

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

        public void AllocateRegisters(GeodeBuilder builder, LifetimeGraph graph)
        {
            var colors = graph.CalculateDSatur();
            foreach (var kv in colors)
            {
                if (kv.Key.NeedsScoreReg)
                {
                    registersInUse.Add(kv.Value);
                    kv.Key.SetValue(builder.Reg(kv.Value));
                }
            }
        }

        public void Render(GeodeBuilder builder)
        {
            var firstBlockRenderer = FirstBlock.GetRenderCtx(builder, this);
            if (UsesStack) firstBlockRenderer.Add(new DataCommand.Modify(GeodeBuilder.RuntimeID, "stack").Append().Value("{}"));

            foreach (var i in registersInUse)
            {
                new StackValue(-1, $"reg_{i}", PrimitiveTypeSpecifier.Int).Store(builder.Reg(i), firstBlockRenderer);
            }

            foreach (var i in blocks)
            {
                i.Render(builder, this);
            }

            builder.Unregister(ExitBlock.Function);

            foreach (var i in registersInUse)
            {
                builder.Reg(i).Store(new StackValue(-1, $"reg_{i}", PrimitiveTypeSpecifier.Int), firstBlockRenderer);
            }

            if (UsesStack) firstBlockRenderer.Add(new DataCommand.Remove(GeodeBuilder.RuntimeID, "stack[-1]"));

            foreach (var i in Tags)
            {
                var tags = builder.Datapack.Tags.GetTag(i, "function").Values;

                if (HasTagPriority) tags.Insert(0, Decl.ID);
                else tags.Add(Decl.ID);
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

        public StackValue GetIsFunctionReturningValue() => new(-1, "returning", PrimitiveTypeSpecifier.Bool);
        public StackValue GetFunctionReturnValue() => GetFunctionReturnValue(Decl.FuncType.ReturnType, UsesStack ? -2 : -1);
        public static StackValue GetFunctionReturnValue(TypeSpecifier type, int depth = -2) => new(depth, "ret", type);

        private class Scope
        {
            public readonly Dictionary<string, Value> Locals = [];
        }
    }
}
