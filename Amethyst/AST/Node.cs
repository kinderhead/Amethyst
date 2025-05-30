using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.AST
{
	public abstract class Node(LocationRange loc)
	{
		public readonly LocationRange Location = loc;
	}
}
