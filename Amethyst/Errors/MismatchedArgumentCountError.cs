using Amethyst.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class MismatchedArgumentCountError(LocationRange loc, int expected, int actual) : AmethystError(loc, $"expected {expected} argument{(expected == 1 ? "" : "s")}, but recieved {actual}")
	{
	}
}
