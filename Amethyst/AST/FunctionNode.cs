using Amethyst.AST.Statements;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
using Amethyst.Errors;
using Datapack.Net.Data;
using Datapack.Net.Function;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Amethyst.AST
{
	[Flags]
	public enum FunctionModifiers
	{
		None = 0,
		NoStack = 1,
		Inline = 2
	}

	public class FunctionNode(LocationRange loc, List<NamespacedID> tags, FunctionModifiers modifiers, AbstractTypeSpecifier ret, NamespacedID id, List<AbstractParameter> parameters, BlockNode body) : Node(loc), IRootChild
	{
		public readonly List<NamespacedID> Tags = tags;
		public readonly FunctionModifiers Modifiers = modifiers;
		public readonly AbstractTypeSpecifier ReturnType = ret;
		public readonly NamespacedID ID = id;
		public readonly List<AbstractParameter> Parameters = parameters;
		public readonly BlockNode Body = body;

		private FunctionTypeSpecifier? funcType = null;
		public FunctionTypeSpecifier GetFunctionType(Compiler ctx) => funcType ??= new(Modifiers, ReturnType.Resolve(ctx), Parameters.Select(i => new Parameter(i.Modifiers, i.Type.Resolve(ctx), i.Name)));

		public bool Compile(Compiler compiler)
		{
			if (Modifiers.HasFlag(FunctionModifiers.Inline))
			{
				if (GetFunctionType(compiler).Parameters.Any(i => i.Modifiers.HasFlag(ParameterModifiers.Macro))) throw new ModifierError(Location, "inline functions cannot have macro arguments");
				return true;
			}

			var ctxs = new List<FunctionContext>();

			if (!SubCompile(compiler, out var ctx1)) return false;
			ctxs.Add(ctx1);

			if (!compiler.Options.KeepLocalsOnStack)
			{
				if (!SubCompile(compiler, out var ctx2, true)) return false;
				ctxs.Add(ctx2);
			}

			PickShortest(compiler, ctxs);

			foreach (var i in Tags)
			{
				compiler.Datapack.Tags.GetTag(i, "function").Values.Add(ID);
			}

			return true;
		}

		private void PickShortest(Compiler compiler, List<FunctionContext> ctxs)
		{
			var keep = ctxs.MinBy(i => i.TotalFunctions.Sum(e => e.Length));
			if (keep is not null)
			{
				compiler.Functions[ID] = keep;
				foreach (var i in keep.TotalFunctions)
				{
					compiler.Register(i);
				}
			}
		}

		private bool SubCompile(Compiler compiler, out FunctionContext ctx, bool keepLocalsOnStack = false)
		{
			var funcType = GetFunctionType(compiler);
			ctx = new FunctionContext(compiler, this, new MCFunction(ID), funcType);
			ctx.KeepLocalsOnStack = ctx.KeepLocalsOnStack || keepLocalsOnStack;
			ctx.PushLocator(Body);

			ctx.Add(new InitFrameInstruction(Body.Location));

			for (var i = 0; i < funcType.Parameters.Length; i++)
			{
				if (funcType.Parameters[i].Modifiers.HasFlag(ParameterModifiers.Macro))
				{
					ctx.RegisterVariable(funcType.Parameters[i].Name, new MacroValue(funcType.Parameters[i].Name, funcType.Parameters[i].Type));
				}
				else
				{
					var stackVal = new StorageValue(Compiler.RuntimeID, $"stack[-1].{funcType.Parameters[i].Name}", funcType.Parameters[i].Type);

					MutableValue val;
					if (funcType.Parameters[i].Type.EffectiveType != NBTType.Int || ctx.KeepLocalsOnStack) val = stackVal;
					else
					{
						val = ctx.AllocScore();
						val.Store(ctx, stackVal);
					}

					ctx.RegisterVariable(Parameters[i].Name, val);
				}
			}

			var ret = Body.CompileWithErrorChecking(ctx);

			if (ctx.CurrentFrame.Instructions.Last() is not ExitFrameInstruction) ctx.Add(new ExitFrameInstruction(Body.Location));

			if (ret) ret = ctx.RequireCompiled();

			return ret;
		}

        public void Process(Compiler ctx)
        {
			if (ctx.Symbols.TryGetValue(ID, out var sym)) throw new RedefinedSymbolError(Location, ID.ToString(), sym.Location);
			ctx.Symbols[ID] = new(ID, Location, new StaticFunctionValue(ID, GetFunctionType(ctx)), this);
		}
    }

	[Flags]
	public enum ParameterModifiers
	{
		None = 0,
		Macro = 1
	}

	public readonly record struct AbstractParameter(ParameterModifiers Modifiers, AbstractTypeSpecifier Type, string Name);
	public readonly record struct Parameter(ParameterModifiers Modifiers, TypeSpecifier Type, string Name);
}
