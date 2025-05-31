using Amethyst.AST.Statements;
using Amethyst.Codegen.IR;
using Datapack.Net.Function;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public class FunctionNode(LocationRange loc, AbstractTypeSpecifier ret, NamespacedID id, BlockNode body) : Node(loc)
	{
		public readonly AbstractTypeSpecifier ReturnType = ret;
		public readonly NamespacedID ID = id;
		public readonly BlockNode Body = body;

		public bool Compile(Compiler compiler)
		{
			var ctx = new FunctionContext(compiler);
			compiler.Functions[ID] = ctx;

			ctx.OnFunctionReturn += i => i.Add(new ExitFrameInstruction(Body.Location));

			ctx.PushFunc(new MCFunction(ID));

			ctx.Add(new InitFrameInstruction(Body.Location));
			var ret = Body.CompileWithErrorChecking(ctx);
			ctx.FireReturn();

			return ret;
		}
	}
}
