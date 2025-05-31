using Amethyst.AST;
using Datapack.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class RedefinedSymbolError(LocationRange loc, string sym, LocationRange src) : DoubleAmethystError(loc, $"symbol \"{sym}\" is already defined", src, "Originally defined at")
	{
	}
}
