using Geode.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Errors
{
	public class VirtualEntityMethodError() : GeodeError("Virtual methods are not supported for entities")
	{
	}
}
