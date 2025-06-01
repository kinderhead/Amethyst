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
	public class FunctionNode(LocationRange loc, List<NamespacedID> tags, AbstractTypeSpecifier ret, NamespacedID id, BlockNode body) : Node(loc)
	{
		public readonly List<NamespacedID> Tags = tags;
		public readonly AbstractTypeSpecifier ReturnType = ret;
		public readonly NamespacedID ID = id;
		public readonly BlockNode Body = body;

		public bool Compile(Compiler compiler)
		{
			var ctx = new FunctionContext(compiler, new MCFunction(ID));
			compiler.Functions[ID] = ctx;

			foreach (var i in Tags)
			{
				compiler.Datapack.Tags.GetTag(i, "function").Values.Add(ID);
			}

			ctx.OnFunctionReturn += i => i.Add(new ExitFrameInstruction(Body.Location));

			ctx.Add(new InitFrameInstruction(Body.Location));
			var ret = Body.CompileWithErrorChecking(ctx);
			ctx.FireReturn();

			return ret;
		}
	}
}
