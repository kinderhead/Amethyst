using Amethyst.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class InvalidTypeError(LocationRange loc, string type) : AmethystError(loc, $"type \"{type}\" is not valid here")
	{
	}
}
