namespace Geode.Errors
{
	public class UndefinedSymbolError(string sym) : GeodeError($"symbol \"{sym}\" does not exist")
	{
	}
}
