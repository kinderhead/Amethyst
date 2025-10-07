namespace Geode.Errors
{
	public class RedefinedSymbolError(string sym, LocationRange src) : DoubleGeodeError($"symbol \"{sym}\" is already defined", src, "Originally defined at")
	{
	}
}
