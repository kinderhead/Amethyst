namespace Geode.Errors
{
	public class MismatchedArgumentCountError(int expected, int actual) : GeodeError($"expected {expected} argument{(expected == 1 ? "" : "s")}, but recieved {actual}")
	{
	}
}
