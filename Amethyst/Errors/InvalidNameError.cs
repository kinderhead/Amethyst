using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class InvalidNameError(string name) : AmethystError($"\"{name}\" is not a valid name here")
	{
	}
}
