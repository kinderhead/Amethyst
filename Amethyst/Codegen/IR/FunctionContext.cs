using Amethyst.AST;
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
	public class FunctionContext(Compiler compiler)
	{
		public readonly Compiler Compiler = compiler;
		public readonly Dictionary<string, LocalSymbol> Variables = [];
		public readonly List<Instruction> Instructions = [];

		private readonly Stack<Frame> frames = [];
		public Frame CurrentFrame { get => frames.Peek(); }

		private readonly Stack<ILocatable> locators = [];
		public ILocatable CurrentLocator { get => locators.Peek(); }

		private int temps = 0;

		public event Action<FunctionContext> OnFunctionReturn;

		public void PushLocator(ILocatable loc) => locators.Push(loc);
		public void PopLocator() => locators.Pop();

		public void PushFunc(MCFunction func)
		{
			Compiler.Register(func);
			frames.Push(new(func));
		}

		public void Add(Instruction insn)
		{
			insn.Ctx = CurrentFrame;
			Instructions.Add(insn);
		}

		public void FireReturn() => OnFunctionReturn?.Invoke(this);
		public void ClearTemps() => temps = 0;

		public StorageValue GetVariable(string name)
		{
			if (Variables.TryGetValue(name, out var val)) return val.Value;
			throw new UndefinedSymbolError(CurrentLocator.Location, name);
		}
		
		public StorageValue AllocTemp(TypeSpecifier type)
		{
			var name = $"$tmp{temps++}";
			return new StorageValue(new(Compiler.RuntimeID), $"stack[-1].{name}", type);
		} 

		public bool Compile()
		{
			var success = true;

			foreach (var i in Instructions)
			{
				if (!Compiler.WrapError(() =>
				{
					i.Build();
					i.Ctx.Function.Add([..i.Commands]);
				})) success = false;
			}

			return success;
		}

		public record class Frame(MCFunction Function);
	}
}
