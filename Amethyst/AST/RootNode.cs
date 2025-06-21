using Amethyst.AST.Statements;
using Amethyst.Codegen;
using Amethyst.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public class RootNode(LocationRange loc, Compiler ctx) : Node(loc)
	{
		public readonly Compiler Ctx = ctx;
		public readonly List<FunctionNode> Functions = [];
		public readonly List<IRootChild> Children = [];

		public bool BuildSymbols()
		{
			var success = true;

			foreach (var i in Children.Concat(Functions))
			{
				if (!Ctx.WrapError(() =>
				{
					i.Process(Ctx);
				})) success = false;
			}

			return success;
		}

		public bool CompileFunctions()
		{
			var success = true;

			foreach (var i in Functions)
			{
				if (!i.Compile(Ctx)) success = false;
			}

			return success;
		}
	}

	public interface IRootChild
	{
		public void Process(Compiler ctx);
	}
}
