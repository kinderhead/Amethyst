using Amethyst.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class UnknownTypeError(LocationRange loc, string type) : AmethystError(loc, $"type \"{type}\" is not declared")
	{
	}
}
