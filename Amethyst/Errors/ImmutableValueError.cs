using Amethyst.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class ImmutableValueError(LocationRange loc) : AmethystError(loc, "value is not mutable")
	{
	}

	public class MutableValueError(LocationRange loc) : AmethystError(loc, "value is mutable")
	{
	}
}
