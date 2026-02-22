using Geode.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Errors
{
	public class ConstantReferenceError() : GeodeError("Cannot assign to a constant reference")
	{
	}
}
