using Geode.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Errors
{
	public class ReservedNameError(string name) : GeodeError($"Name \"{name}\" is reserved")
	{
	}
}
