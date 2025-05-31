using Amethyst.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class UndefinedSymbolError(LocationRange loc, string sym) : AmethystError(loc, $"symbol \"{sym}\" does not exist")
	{
	}
}
