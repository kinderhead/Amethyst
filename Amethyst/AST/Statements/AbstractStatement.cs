using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST.Statements
{
	public abstract class AbstractStatement(LocationRange loc) : Node(loc)
	{
	}
}
