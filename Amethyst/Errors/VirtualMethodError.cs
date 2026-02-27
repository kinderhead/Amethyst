using Geode.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Errors
{
	public class VirtualMethodError() : GeodeError("Virtual methods are not supported in this context")
	{
	}
}
