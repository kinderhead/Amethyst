namespace Geode.Errors
{
	public class TargetSelectorArgumentError(string arg) : GeodeError($"Invalid target selector argument \"{arg}\"")
	{

	}

	public class TargetSelectorMacroArgumentError(string arg) : GeodeError($"Dynamic argument \"{arg}\" is not valid in this context")
	{

	}
}
