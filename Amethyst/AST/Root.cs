using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public class RootNode(LocationRange loc) : Node(loc)
	{
		public readonly List<FunctionNode> Functions = [];
	}
}
