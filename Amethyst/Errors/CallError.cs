using Amethyst.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class CallError(LocationRange loc) : AmethystError(loc, "value is not callable")
	{
	}
}
