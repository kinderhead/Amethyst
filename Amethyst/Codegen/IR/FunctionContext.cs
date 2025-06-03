using Amethyst.AST;
using Amethyst.AST.Statements;
using Amethyst.Errors;
using Datapack.Net.Function;
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
		public readonly Dictionary<string, LocalSymbol> Variables = [];
		public readonly List<ScoreValue> LocalScores = [];
		public readonly MCFunction MainFunction;

		private readonly List<Frame> totalFrames = [];
		private readonly Stack<Frame> frames = [];
		public Frame CurrentFrame { get => frames.Peek(); }

		private readonly Stack<ILocatable> locators = [];
		public ILocatable CurrentLocator { get => locators.Peek(); }

		private int tempStack = 0;
		private int tempScore = 0;
		private int localScores = 0;
		private int elseScores = 0;
		private bool compiled = false;
		private bool compiledSuccess = true;

		public FunctionContext(Compiler compiler, MCFunction func)
		{
			Compiler = compiler;
			MainFunction = func;
			PushFunc(func);
		}

		public event Action<FunctionContext> OnFunctionReturn;

		public void PushLocator(ILocatable loc) => locators.Push(loc);
		public void PopLocator() => locators.Pop();

		public void PushFunc(MCFunction func)
		{
			Compiler.Register(func);
			var frame = new Frame(func, this);
			totalFrames.Add(frame);
			frames.Push(frame);
		}

		public void PopFunc() => frames.Pop();

		public SubFunction SubFunc(Statement stmt)
		{
			var func = new MCFunction(new(CurrentFrame.Function.ID.Namespace, $"zz_internal/{Compiler.RandomString}"));
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

		public void FireReturn() => OnFunctionReturn?.Invoke(this);
		public void ClearTemps()
		{
			tempStack = 0;
			tempScore = 0;
		}

		public Value GetVariable(string name)
		{
			if (GetVariableNoThrow(name) is Value v) return v;
			throw new UndefinedSymbolError(CurrentLocator.Location, name);
		}

		public Value? GetVariableNoThrow(string name)
		{
			if (Variables.TryGetValue(name, out var val)) return val.Value;
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
		
		public StorageValue AllocTemp(TypeSpecifier type) => new(new(Compiler.RuntimeID), $"stack[-1].$tmp{tempStack++}", type);

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

		public class Frame(MCFunction function, FunctionContext ctx)
		{
			public readonly MCFunction Function = function;
			public readonly FunctionContext Ctx = ctx;
			public readonly List<Instruction> Instructions = [];
			
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
		}
	}
}
