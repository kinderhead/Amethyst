using Amethyst.AST;
using Amethyst.Codegen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amethyst.Errors
{
	public class InvalidCastError(LocationRange loc, TypeSpecifier oldType, TypeSpecifier newType) : AmethystError(loc, $"{oldType} cannot be casted to {newType}")
	{
	}
}
