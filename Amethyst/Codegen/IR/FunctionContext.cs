using Amethyst.AST;
using Amethyst.AST.Statements;
using Amethyst.Codegen.Functions;
using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Function.Commands;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Amethyst.Codegen.IR
{
	public class FunctionContext
	{
		public readonly Compiler Compiler;
		public readonly FunctionNode Node;
		public readonly List<ScoreValue> LocalScores = [];
		public readonly MCFunction MainFunction;
		public readonly List<MCFunction> TotalFunctions = [];
		public readonly FunctionTypeSpecifier FunctionType;

		public bool KeepLocalsOnStack = false;

		public bool UsesStack { get; private set; } = false;

		private readonly List<Frame> totalFrames = [];
		private readonly Stack<Frame> frames = [];
		public Frame CurrentFrame { get => frames.Peek(); }

		private readonly Stack<ILocatable> locators = [];
		public ILocatable CurrentLocator { get => locators.Peek(); }
		public bool HasMacros => FunctionType.Parameters.Any(i => i.Modifiers.HasFlag(ParameterModifiers.Macro));

		public int InlineDepth { get; private set; } = 0;
		public MutableValue ReturnValue;

		private Dictionary<string, LocalSymbol> variables = [];

		private int tempStack = 0;
		private int tempScore = 0;
		private int localScores = 0;
		private int elseScores = 0;
		private bool compiled = false;
		private bool compiledSuccess = true;
		private string variablePrefix = "";

		public FunctionContext(Compiler compiler, FunctionNode node, MCFunction func, FunctionTypeSpecifier funcType)
		{
			Compiler = compiler;
			Node = node;
			MainFunction = func;
			FunctionType = funcType;
			KeepLocalsOnStack = compiler.Options.KeepLocalsOnStack;
			ReturnValue = new StorageValue(Compiler.RuntimeID, "return", FunctionType.ReturnType);
			PushFunc(func);
		}

		public void PushLocator(ILocatable loc) => locators.Push(loc);
		public void PopLocator() => locators.Pop();

		public void PushFunc(MCFunction func)
		{
			TotalFunctions.Add(func);
			var frame = new Frame(func, this, frames.Count == 0);
			totalFrames.Add(frame);
			if (frames.Count != 0) CurrentFrame.Subframes.Add(frame);
			frames.Push(frame);
		}

		public void PopFunc() => frames.Pop();

		public NamespacedID NewInternalID() => new(CurrentFrame.Function.ID.Namespace, $"zz_internal/{Compiler.RandomString}");
		public SubFunction SubFunc(Statement stmt)
		{
			var func = new MCFunction(NewInternalID());
			PushFunc(func);
			var frame = CurrentFrame;
			stmt.Compile(this);
			PopFunc();
			return new(frame, func.ID, func);
		}

		public void Add(Instruction insn)
		{
			insn.Ctx = CurrentFrame;
			CurrentFrame.Instructions.Add(insn);
		}

		public void ClearTemps()
		{
			tempStack = 0;
			tempScore = 0;
		}

		public string GetStackVariablePath(string name) => $"stack[-1].{variablePrefix}{name}";

		public void RegisterVariable(string name, Value val)
		{
			if (variables.TryGetValue(name, out var sym)) throw new RedefinedSymbolError(CurrentLocator.Location, name, sym.Location);
			variables[name] = new(name, CurrentLocator.Location, val);
			if (val is StorageValue s && s.Path.StartsWith("stack")) UsesStack = true;
		}

		public Value GetVariable(string name)
		{
			if (GetVariableNoThrow(name) is Value v) return v;
			throw new UndefinedSymbolError(CurrentLocator.Location, name);
		}

		public Value? GetVariableNoThrow(string name)
		{
			if (variables.TryGetValue(name, out var val)) return val.Value;
			else
			{
				if (!name.Contains(':'))
				{
					if (GetVariableNoThrow($"amethyst:{name}") is Value v) return v;
					name = $"{CurrentFrame.Function.ID.Namespace}:{name}";
				}

				if (Compiler.CompileTimeFunctions.TryGetValue(name, out var func)) return func;
				else if (Compiler.Symbols.TryGetValue(name, out var gval)) return gval.Value;
			}
			return null;
		}

		public void WithNewVariables(Action cb)
		{
			var oldVars = variables;
			var oldPrefix = variablePrefix;
			variables = [];
			variablePrefix = $"${InlineDepth++}_";
			try
			{
				cb();
			}
			finally
			{
				variables = oldVars;
				variablePrefix = oldPrefix;
				InlineDepth--;
			}
		}

		public StorageValue AllocTemp(TypeSpecifier type)
		{
			UsesStack = true;
			return new(new(Compiler.RuntimeID), $"stack[-1].$tmp{tempStack++}", type);
		}

		public ScoreValue AllocTempScore()
		{
			var score = new Score($"_tmp{tempScore++}", "dummy");
			Compiler.Register(score);
			return new(Compiler.RuntimeEntity, score);
		}

		public ScoreValue AllocScore()
		{
			var score = new Score($"_local{localScores++}", "dummy");
			Compiler.Register(score);
			var val = new ScoreValue(Compiler.RuntimeEntity, score);
			LocalScores.Add(val);
			return val;
		}

		public ScoreValue AllocElseScore()
		{
			var score = new Score($"_else{elseScores++}", "dummy");
			Compiler.Register(score);
			var val = new ScoreValue(Compiler.RuntimeEntity, score);
			LocalScores.Add(val);
			return val;
		}

		public ScoreValue Constant(int num) => Compiler.Constant(num);

		public Value Call(NamespacedID id, IList<Value> rawArgs, StorageValue? dest = null)
		{
			if (!Compiler.Symbols.TryGetValue(id, out var sym) || sym.Value is not StaticFunctionValue f) throw new UndefinedSymbolError(CurrentLocator.Location, id.ToString());
			if (f is CompileTimeFunction c)
			{
				var ret = c.Execute(this, rawArgs);
				dest?.Store(this, ret);
				return ret;
			}

			if (f.FuncType.Parameters.Length != rawArgs.Count) throw new MismatchedArgumentCountError(CurrentLocator.Location, f.FuncType.Parameters.Length, rawArgs.Count);

			for (var i = 0; i < rawArgs.Count; i++)
			{
				if (!rawArgs[i].Type.IsAssignableTo(f.FuncType.Parameters[i].Type)) throw new InvalidCastError(CurrentLocator.Location, rawArgs[i].Type, f.FuncType.Parameters[i].Type);
			}

			var retVal = new StorageValue(Compiler.RuntimeID, "return", f.FuncType.ReturnType);

			if (f.FuncType.Modifiers.HasFlag(FunctionModifiers.Inline))
			{
				retVal = dest ?? retVal;

				WithNewVariables(() =>
				{
					for (int i = 0; i < rawArgs.Count; i++)
					{
						RegisterVariable(f.FuncType.Parameters[i].Name, rawArgs[i]);
					}

					var oldRetVal = ReturnValue;
					ReturnValue = retVal;
					((FunctionNode)sym.Node).Body.Compile(this);
					ReturnValue = oldRetVal;
				});

				return retVal.Type is VoidTypeSpecifier ? new VoidValue() : retVal;
			}
			else
			{
				var args = new Dictionary<string, Value>();
				var macros = new Dictionary<string, Value>();

				if (f.FuncType.Parameters.Length != 0)
				{
					for (var i = 0; i < rawArgs.Count; i++)
					{
						if (f.FuncType.Parameters[i].Modifiers.HasFlag(ParameterModifiers.Macro)) macros[f.FuncType.Parameters[i].Name] = rawArgs[i];
						else args[f.FuncType.Parameters[i].Name] = rawArgs[i];
					}
				}

				Add(new CallInstruction(CurrentLocator.Location, f.ID, args, macros));

				if (f.FuncType.ReturnType is VoidTypeSpecifier) return new VoidValue();

				dest?.Store(this, retVal);
				return dest ?? retVal;
			}
		}

		public bool RequireCompiled()
		{
			if (!compiled)
			{
				foreach (var i in totalFrames)
				{
					if (!i.RequireCompiled()) compiledSuccess = false;
				}
				compiled = true;
			}

			return compiledSuccess;
		}

		public class Frame(MCFunction function, FunctionContext ctx, bool isBase)
		{
			public readonly MCFunction Function = function;
			public readonly FunctionContext Ctx = ctx;
			public readonly bool IsBase = isBase;
			public readonly List<Instruction> Instructions = [];
			public readonly List<Frame> Subframes = [];
			
			public List<Command> Commands
			{
				get {
					var cmds = new List<Command>();
					foreach (var i in Instructions)
					{
						cmds.AddRange(i.Commands);
					}
					return cmds;
				}
			}

			private bool compiled = false;
			private bool compiledSuccess = true;

			public bool RequireCompiled()
			{
				if (!compiled)
				{
					foreach (var i in Instructions)
					{
						if (!Ctx.Compiler.WrapError(() =>
						{
							i.Build();
							i.Ctx.Function.Add([.. i.Commands]);
						})) compiledSuccess = false;
					}
					compiled = true;
				}

				return compiledSuccess;
			}

			public int CommandCount()
			{
				int count = 0;
				foreach (var i in Instructions) { count += i.Commands.Count; }
				return count;
			}

			public bool HasInstruction<T>() where T : Instruction
			{
				foreach (var i in Instructions)
				{
					if (i is T) return true;
				}

				foreach (var i in Subframes)
				{
					if (i.HasInstruction<T>()) return true;
				}

				return false;
			}
		}
	}
}
