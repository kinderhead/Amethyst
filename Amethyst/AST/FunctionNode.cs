using Amethyst.AST.Statements;
using Amethyst.Codegen;
using Amethyst.Codegen.IR;
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
	public class FunctionNode(LocationRange loc, List<NamespacedID> tags, AbstractTypeSpecifier ret, NamespacedID id, List<Parameter> parameters, BlockNode body) : Node(loc)
	{
		public readonly List<NamespacedID> Tags = tags;
		public readonly AbstractTypeSpecifier ReturnType = ret;
		public readonly NamespacedID ID = id;
		public readonly List<Parameter> Parameters = parameters;
		public readonly BlockNode Body = body;

		private FunctionTypeSpecifier? funcType = null;
		public FunctionTypeSpecifier GetFunctionType(Compiler ctx) => funcType ??= new(ReturnType.Resolve(ctx), Parameters.Select(i => i.Type.Resolve(ctx)));

		public bool Compile(Compiler compiler)
		{
			var ctxs = new List<FunctionContext>();

			if (!SubCompile(compiler, out var ctx1)) return false;
			ctxs.Add(ctx1);

			if (!compiler.Options.KeepLocalsOnStack)
			{
				if (!SubCompile(compiler, out var ctx2, true)) return false;
				ctxs.Add(ctx2);
			}

			PickShortest(compiler, ctxs);

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
			ctx = new FunctionContext(compiler, new MCFunction(ID), funcType);
			ctx.KeepLocalsOnStack = ctx.KeepLocalsOnStack || keepLocalsOnStack;
			ctx.PushLocator(Body);

			foreach (var i in Tags)
			{
				compiler.Datapack.Tags.GetTag(i, "function").Values.Add(ID);
			}

			ctx.Add(new InitFrameInstruction(Body.Location));

			for (var i = 0; i < funcType.Parameters.Length; i++)
			{
				var stackVal = new StorageValue(new(Compiler.RuntimeID), $"stack[-1].$arg{i}", funcType.Parameters[i]);

				MutableValue val;
				if (funcType.Parameters[i].Type != NBTType.Int || ctx.KeepLocalsOnStack) val = stackVal;
				else
				{
					val = ctx.AllocScore();
					val.Store(ctx, stackVal);
				}

				ctx.Variables[Parameters[i].Name] = new(Parameters[i].Name, val.Type, Location, val);
			}

			var ret = Body.CompileWithErrorChecking(ctx);

			if (ctx.CurrentFrame.Instructions.Last() is not ExitFrameInstruction) ctx.Add(new ExitFrameInstruction(Body.Location));

			if (ret) ret = ctx.RequireCompiled();

			return ret;
		}
	}

	public readonly record struct Parameter(AbstractTypeSpecifier Type, string Name);
}
