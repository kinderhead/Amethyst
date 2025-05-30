using Amethyst.AST.Statements;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public class FunctionNode(LocationRange loc, AbstractTypeDeclaration ret, NamespacedID name, BlockNode body) : Node(loc)
	{
		public readonly AbstractTypeDeclaration ReturnType = ret;
		public readonly NamespacedID Name = name;
		public readonly BlockNode Body = body;
	}
}
