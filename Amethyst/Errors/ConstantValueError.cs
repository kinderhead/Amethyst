using Geode.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Errors
{
	public class ConstantValueError() : GeodeError("Value is not constant")
	{
	}
}
