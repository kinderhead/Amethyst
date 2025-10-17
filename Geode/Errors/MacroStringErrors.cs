namespace Geode.Errors
{
	public class MacroStringError() : GeodeError("Only string literals may be passed into macro string arguments")
	{

	}

	public class MacroStringSubFunctionError() : GeodeError("Macro strings cannot be passed through to sub functions like conditionals")
	{

	}
}
