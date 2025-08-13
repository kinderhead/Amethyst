namespace Amethyst.Errors
{
	public class MismatchedArgumentCountError(int expected, int actual) : AmethystError($"expected {expected} argument{(expected == 1 ? "" : "s")}, but recieved {actual}")
	{
	}
}
