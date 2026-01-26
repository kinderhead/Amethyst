using System;
using System.Collections.Generic;
using System.Text;

namespace Geode.Errors
{
	public class NotInLoopError() : GeodeError("Cannot use this statement outside of a loop")
	{
	}
}
