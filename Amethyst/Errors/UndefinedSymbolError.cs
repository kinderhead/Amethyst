namespace Amethyst.Errors
{
	public class UndefinedSymbolError(string sym) : AmethystError($"symbol \"{sym}\" does not exist")
	{
	}
}
