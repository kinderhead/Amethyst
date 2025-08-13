using Amethyst.AST;

namespace Amethyst.Errors
{
	public class RedefinedSymbolError(string sym, LocationRange src) : DoubleAmethystError($"symbol \"{sym}\" is already defined", src, "Originally defined at")
	{
	}
}
