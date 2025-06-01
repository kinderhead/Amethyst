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

		public bool BuildSymbols()
		{
			var success = true;

			foreach (var i in Functions)
			{
				if (!Ctx.WrapError(() =>
				{
					if (Ctx.Symbols.TryGetValue(i.ID, out var sym)) throw new RedefinedSymbolError(i.Location, i.ID.ToString(), sym.Location);
					var type = new FunctionTypeSpecifier(i.ReturnType.Resolve(Ctx));
					Ctx.Symbols[i.ID] = new(i.ID, type, i.Location, new StaticFunctionValue(i.ID, type));
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
}
