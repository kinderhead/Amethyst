namespace Geode.Errors
{
	public class TargetSelectorArgumentError(string arg) : GeodeError($"Invalid target selector argument \"{arg}\"")
	{

	}

	public class TargetSelectorEmptyArgumentError(string arg) : GeodeError($"Target selector argument \"{arg}\" cannot be blank")
	{

	}

	public class TargetSelectorMacroArgumentError(string arg) : GeodeError($"Dynamic argument \"{arg}\" is not valid in this context")
	{

	}

	public class TargetSelectorNegatedLiteralError(string literal) : GeodeError($"Literal \"{literal}\" contains an exclamation mark. Move it outside the string")
	{

	}

	public class TargetSelectorInvalidSortError(string sort) : GeodeError($"Invalid sort option \"{sort}\"")
	{

	}

	public class TargetSelectorInvalidGamemodeError(string sort) : GeodeError($"Invalid gamemode \"{sort}\"")
	{

	}
}
