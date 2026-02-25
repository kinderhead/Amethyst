using Geode.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Errors
{
	public class InvalidBaseClassError(string type) : GeodeError($"Type {type} is not a valid base class in this context")
	{
	}
}
