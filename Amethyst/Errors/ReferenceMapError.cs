using Geode.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amethyst.Errors
{
	public class ReferenceMapError() : GeodeError("Native maps do not support garbage collection and cannot store references")
	{
	}
}
