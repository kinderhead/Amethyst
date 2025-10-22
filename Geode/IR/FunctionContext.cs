using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using Geode.Errors;
using Geode.IR.Instructions;
using Geode.IR.Passes;
using Geode.Types;
using Geode.Values;
using System.Text;

namespace Geode.IR
{
	public class FunctionContext
	{
		public readonly ICompiler Compiler;
		public readonly FunctionValue Decl;
		public readonly IEnumerable<NamespacedID> Tags;

		public readonly Stack<LocationRange> LocationStack = [];

		public Block FirstBlock => blocks.First();
		public readonly Block ExitBlock;
		public Block CurrentBlock { get; private set; }

		public bool IsFinished => CurrentBlock == ExitBlock;

		// If there's more than one block, then stack[-1].returning is used
		public bool UsesStack => AllLocals.Any(i => i is StorageValue) || registersInUse.Count != 0 || tmpStackVars != 0 || Blocks.Count > 1;
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

		public IEnumerable<IValue> AllLocals => totalScopes.SelectMany(i => i.Locals.Values);

		public FunctionContext(ICompiler compiler, FunctionValue decl, IEnumerable<NamespacedID> tags, bool hasTagPriority = false)
		{
			Compiler = compiler;
			Decl = decl;
			Tags = tags;
			IsMacroFunction = Decl.FuncType.IsMacroFunction;
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
					RegisterLocal(i.Name, new StackValue(-2, compiler.IR.RuntimeID, $"args.{i.Name}", i.Type));
				}
			}
		}

		public FunctionContext(ICompiler compiler, FunctionValue decl) : this(compiler, decl, []) { }

		public void PushScope()
		{
			var scope = new Scope();
			totalScopes.Add(scope);
			activeScopes.Push(scope);
		}

		public void PopScope() => activeScopes.Pop();

		public IValue GetVariable(string name) => GetVariableOrNull(name) ?? throw new UndefinedSymbolError(name);

		public IValue? GetVariableOrNull(string name)
		{
			foreach (var i in activeScopes.Reverse())
			{
				if (i.Locals.TryGetValue(name, out var variable))
				{
					return variable;
				}
			}

			if (name.Contains(':') && GetGlobal(new NamespacedID(name)) is IValue v)
			{
				return v;
			}
			else if (GetGlobalWalk(Decl.ID.GetContainingFolder(), name) is IValue v2)
			{
				return v2;
			}
			else if (GetGlobal(new("minecraft", name)) is IValue v3)
			{
				return v3;
			}
			else if (GetGlobal(new("builtin", name)) is IValue v4)
			{
				return v4;
			}

			return null;
		}

		public IValue? GetGlobal(NamespacedID id) => Compiler.IR.GetGlobal(id);
		public IValue? GetGlobalWalk(string baseNamespace, string name) => Compiler.IR.GetGlobalWalk(baseNamespace, name);

		public IValue? GetConstructorOrNull(TypeSpecifier type) => Compiler.IR.GetConstructorOrNull(type);

		public Variable RegisterLocal(string name, TypeSpecifier type)
		{
			var val = new Variable(name, Compiler.IR.RuntimeID, "frame", activeScopes.Count - 1, type);
			RegisterLocal(name, val);
			return val;
		}

		public void RegisterLocal(string name, IValue val) => activeScopes.Peek().Locals[name] = val;

		public StackValue Temp(TypeSpecifier type) => new(-1, Compiler.IR.RuntimeID, $"tmp{tmpStackVars++}", type);

		public ValueRef Add(Instruction insn, string? customName = null)
		{
			if (IsFinished)
			{
				throw new InvalidOperationException("Function is finished");
			}

			if (LocationStack.Count != 0)
			{
				insn.Location = LocationStack.Peek();
			}

			var val = CurrentBlock.Add(insn);
			val.Name = customName!;
			return val;
		}

		public ValueRef ImplicitCast(ValueRef val, TypeSpecifier type)
		{
			if (ImplicitCastOrNull(val, type) is ValueRef ret)
			{
				return ret;
			}

			throw new InvalidTypeError(val.Type.ToString(), type.ToString());
		}

		public ValueRef? ImplicitCastOrNull(ValueRef val, TypeSpecifier type)
		{
			if (val.Type == type)
			{
				return val;
			}
			else if (type is VarType)
			{
				return val;
			}
			else if (type is AnyType)
			{
				return val.SetType(type);
			}
			else if (val.Type is AnyType)
			{
				return val.SetType(type);
			}
			else if (val.Type.CastFromOverload(val, type, this) is ValueRef cast)
			{
				return cast.SetType(type);
			}
			else if (type.CastToOverload(val, this) is ValueRef cast2)
			{
				return cast2.SetType(type);
			}
			else if (val.Type.Implements(type))
			{
				return val.SetType(type);
			}
			else if (val.Value is LiteralValue literal && type is PrimitiveType)
			{
				if (literal.Value.NumberType is Datapack.Net.Data.NBTNumberType && type.EffectiveNumberType is Datapack.Net.Data.NBTNumberType destType)
				{
					return new LiteralValue(literal.Value.Cast(destType));
				}
			}
			else if ((type == PrimitiveType.Double || type == PrimitiveType.Float) && ImplicitCastOrNull(val, PrimitiveType.Int) is ValueRef toFloat)
			{
				return toFloat.SetType(type);
			}

			return null;
		}

		public ValueRef ExplicitCast(ValueRef val, TypeSpecifier type)
		{
			if (ImplicitCastOrNull(val, type) is ValueRef ret)
			{
				return ret;
			}
			else if (val.Type.ExplicitCastFromOverload(val, type, this) is ValueRef cast)
			{
				return cast.SetType(type);
			}
			else if (type.EffectiveType == Datapack.Net.Data.NBTType.Int)
			{
				return Add(new LoadInsn(val, type)).SetType(type);
			}
			else if (type.Implements(val.Type))
			{
				return val.SetType(type);
			}

			throw new InvalidTypeError(val.Type.ToString(), type.ToString());
		}

		public ValueRef Call(NamespacedID id, params ValueRef[] args) => Call(GetGlobal(id) as FunctionValue ?? throw new UndefinedSymbolError(id.ToString()), args);

		public ValueRef Call(FunctionValue f, params ValueRef[] args) => Add(new CallInsn(f, PrepArgs(f.FuncType, args)));

		public IEnumerable<ValueRef> PrepArgs(FunctionType type, params ValueRef[] args)
		{
			if (type.Parameters.Length != args.Length)
			{
				throw new MismatchedArgumentCountError(type.Parameters.Length, args.Length);
			}

			return args.Zip(type.Parameters).Select(i => ImplicitCast(i.First, i.Second.Type));
		}

		public void Finish()
		{
			if (CurrentBlock.Instructions.Count == 0 || !CurrentBlock.Instructions.Last().IsReturn)
			{
				throw new InvalidOperationException("Last block in function must have a return instruction");
			}

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
		/// Branch current block without an else statement.
		/// </summary>
		/// <param name="cond">Condition</param>
		/// <param name="label">Label name</param>
		/// <param name="ifTrue">If true action</param>
		/// <returns>True block</returns>
		public Block Branch(ExecuteChain cond, string label, Action ifTrue)
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

		public (Block trueBlock, Block falseBlock) Branch(ExecuteChain cond, string label, Action ifTrue, Action ifFalse)
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

		public Block Loop(Func<ExecuteChain> cond, string label, Action loop)
		{
			label = GetNewLabelName(label);

			var startingBlock = CurrentBlock;
			var loopBlock = new Block($"{label}.loop", GetNewInternalID(), this);
			var endBlock = new Block($"{label}.end", GetNewInternalID(), this);

			blocks.Add(loopBlock);
			blocks.Add(endBlock);

			startingBlock.Link(loopBlock);
			startingBlock.Link(endBlock);
			loopBlock.Link(loopBlock);
			loopBlock.Link(endBlock);

			Add(new BranchInsn(cond(), loopBlock, endBlock));

			CurrentBlock = loopBlock;
			loop();
			Add(new BranchInsn(cond(), loopBlock, endBlock));

			CurrentBlock = endBlock;

			return loopBlock;
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
			if (UsesStack)
			{
				firstBlockRenderer.Add(new DataCommand.Modify(builder.RuntimeID, "stack").Append().Value("{}"));
			}

			foreach (var i in registersInUse)
			{
				new StackValue(-1, builder.RuntimeID, $"reg_{i}", PrimitiveType.Int).Store(builder.Reg(i), firstBlockRenderer);
			}

			foreach (var i in blocks)
			{
				i.Render(builder, this);
			}

			builder.Unregister(ExitBlock.Function);

			foreach (var i in registersInUse)
			{
				builder.Reg(i).Store(new StackValue(-1, builder.RuntimeID, $"reg_{i}", PrimitiveType.Int), firstBlockRenderer);
			}

			if (UsesStack)
			{
				firstBlockRenderer.Add(new DataCommand.Remove(builder.RuntimeID, "stack[-1]"));
			}

			foreach (var i in Tags)
			{
				var tags = builder.Datapack.Tags.GetTag(i, "function").Values;

				if (HasTagPriority)
				{
					tags.Insert(0, Decl.ID);
				}
				else
				{
					tags.Add(Decl.ID);
				}
			}
		}

		public string Dump()
		{
			var builder = new StringBuilder();

			builder.AppendLine(((FunctionType)Decl.Type).ToString(Decl.ID.ToString()) + " {");

			var valCounter = 0;
			Dictionary<IInstructionArg, int> valueMap = [];

			foreach (var i in blocks)
			{
				if (i.Instructions.Count == 0)
				{
					continue;
				}

				builder.AppendLine(i.Dump(val =>
				{
					if (val.Name != "")
					{
						return val.Name;
					}

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

		public NamespacedID GetNewInternalID() => new(Decl.ID.Namespace, $"{GeodeBuilder.InternalPath}/{GeodeBuilder.RandomString}");

		public StackValue GetIsFunctionReturningValue() => new(-1, Compiler.IR.RuntimeID, "returning", PrimitiveType.Bool);
		public StackValue GetFunctionReturnValue() => GetFunctionReturnValue(Decl.FuncType.ReturnType, UsesStack ? -2 : -1);
		public StackValue GetFunctionReturnValue(TypeSpecifier type, int depth = -2) => new(depth, Compiler.IR.RuntimeID, "ret", type);

		private class Scope
		{
			public readonly Dictionary<string, IValue> Locals = [];
		}
	}
}
