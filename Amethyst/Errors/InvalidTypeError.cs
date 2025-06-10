using Amethyst.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class InvalidTypeError : AmethystError
	{
		public InvalidTypeError(LocationRange loc, string type) : base(loc, $"type \"{type}\" is not valid here")
		{
		}

		public InvalidTypeError(LocationRange loc, string type, string expected) : base(loc, $"expected \"{expected}\" but got type \"{type}\"")
		{
		}
	}
}
