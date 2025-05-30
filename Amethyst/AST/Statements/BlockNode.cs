using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public class BlockNode(LocationRange loc) : AbstractStatement(loc)
	{
		public readonly List<AbstractStatement> Statements = [];
	}
}
